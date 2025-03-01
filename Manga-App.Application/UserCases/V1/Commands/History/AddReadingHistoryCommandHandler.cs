

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.History.Command;

namespace MangaApp.Application.UserCases.V1.Commands.History;

public sealed class AddReadingHistoryCommandHandler : ICommandHandler<AddReadingHistoryCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    public AddReadingHistoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<Success>> Handle(AddReadingHistoryCommand request, CancellationToken cancellationToken)
    {
        bool chapterExists = await _unitOfWork
         .ChapterRepository
         .FindAll(x => x.Id == request.ChapterId && x.MangaId == request.MangaId, tracking: false)
         .AnyAsync(cancellationToken);

        if (!chapterExists)
        {
            return Error.Failure(code: "ChapterNotFound", description: "Chapter not found.");
        }

        var history = await _unitOfWork
            .HistoryRepository
            .FindAll(x=> x.UserId == request.UserId && x.MangaId == request.MangaId && x.ChapterId == request.ChapterId, tracking: false)
            .SingleOrDefaultAsync();

        if(history is not null)
        {
            history.LastReadAt = DateTime.UtcNow;
            _unitOfWork.HistoryRepository.Update(history);
        }
        else
        {
            var newHistory = new Domain.Entities.History(request.UserId,request.MangaId, request.ChapterId);
            _unitOfWork.HistoryRepository.Add(newHistory);
        }
        await _unitOfWork.SaveChangesAsync();

        return ResultType.Success;
    }
}
