

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Comment.Query;
using static MangaApp.Contract.Services.V1.Comment.Response;

namespace MangaApp.Application.UserCases.V1.Queries.Comment;

public sealed class GetCommentQueryHandler : IQueryHandler<GetCommentQuery, Pagination<CommentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetCommentQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Pagination<CommentResponse>>> Handle(GetCommentQuery request, CancellationToken cancellationToken)
    {
        var commentSelect =
             _unitOfWork
            .CommentRepository
            .FindAll(x => x.Type == request.Type && x.CommentTypeId == request.TargetId && x.ParentId == null)
            .Select(x => new CommentResponse {
                Id = x.Id,
                Type = x.Type,
                Action = x.Action,
                TargetId = x.CommentTypeId,
                Author = new CommentUser
                {
                    Id = x.UserId,
                    DisplayName = x.User.DisplayName,
                    Email = x.User.Email,
                    Avatar = x.User.Avatar
                },
                Content = x.Content,
                ReplyCount = _unitOfWork.GetDbContext()
                .Set<Domain.Entities.Comment>()
                .Where(y => y.Left > x.Left && y.Right < x.Right && y.ParentId != null)
                .Count(),
                CreatedAt = x.CreatedDate,
                UpdatedAt = x.ModifiedDate
            });
        var commentResponse = await commentSelect
            .OrderByDescending(x=> x.ReplyCount)
                .ThenByDescending(x => x.CreatedAt)
            .Skip((request.PageIndex-1)* request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        var commentTotal = await commentSelect.CountAsync();
        return Pagination<CommentResponse>.Create(commentResponse, commentTotal,request.PageIndex, request.PageIndex );

    }
}
