

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Contract.Shares.Errors;
using MangaApp.Domain.Entities;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Comment.Command;

namespace MangaApp.Application.UserCases.V1.Commands.Comment;

public class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    public CreateCommentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var commentAction = request.ParentId is null ? CommentAction.Commented : CommentAction.Replied;
        var comment = new Domain.Entities.Comment(request.UserId, commentAction, request.Type, request.TypeId, request.Content, request.ParentId);
        int newRight, newLeft;
        try
        {

            if (request.ParentId.HasValue)
            {
                var parent = await _unitOfWork.CommentRepository.FindSingleAsync(c => c.Id == request.ParentId.Value);
                if (parent == null)
                    return Error.Failure(description: "Comment not found");

                var rightValue = parent.Right;
                await UpdateBoundariesForInsertAsync(rightValue, request.Type, request.TypeId);
                newRight = rightValue + 1;
                newLeft = rightValue;
            }
            else
            {
                var maxRight = await _unitOfWork.CommentRepository
                .FindAll(x => x.Type == request.Type && x.CommentTypeId == request.TypeId)
                    .MaxAsync(c => (int?)c.Right) ?? 0;
                newLeft = maxRight + 1;
                newRight = newLeft + 1;
            }

            comment.Left = newLeft;
            comment.Right = newRight;
            _unitOfWork.CommentRepository.Add(comment);
            await _unitOfWork.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }

        return ResultType.Success;
    }
    private async Task UpdateBoundariesForInsertAsync(int rightBoundary, CommentType type, Guid TypeId)
    {
        // Increase the left and right boundaries of the existing comments
        var commentsToUpdate = _unitOfWork.CommentRepository
            .FindAll(c => c.Type == type && c.CommentTypeId == TypeId, tracking: true)
            .ToList();

        foreach (var comment in commentsToUpdate)
        {
            if (comment.Right >= rightBoundary)
            {
                comment.Right += 2; // Shift right by 2 for new node
            }
            if (comment.Left > rightBoundary)
            {
                comment.Left += 2; // Shift left by 2 for new node
            }
        }

        // Update the modified comments in the context
        await _unitOfWork.SaveChangesAsync();
    }
}
