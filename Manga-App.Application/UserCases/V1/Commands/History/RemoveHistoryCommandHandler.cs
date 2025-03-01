using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.History.Command;

namespace MangaApp.Application.UserCases.V1.Commands.History;

public class RemoveHistoryCommandHandler : ICommandHandler<RemoveHistoryCommand, Deleted>
{
    private readonly IUnitOfWork _unitOfWork;
    public RemoveHistoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<Deleted>> Handle(RemoveHistoryCommand request, CancellationToken cancellationToken)
    {
        var mangaHistories = await _unitOfWork
            .HistoryRepository
            .FindAll(x=> x.UserId == request.UserId)
            .ToListAsync(cancellationToken);
        _unitOfWork.HistoryRepository.RemoveMultiple(mangaHistories);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultType.Deleted;
    }
}
