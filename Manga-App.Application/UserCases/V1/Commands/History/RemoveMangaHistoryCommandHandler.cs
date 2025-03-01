using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.History.Command;

namespace MangaApp.Application.UserCases.V1.Commands.History;

public class RemoveMangaHistoryCommandHandler : ICommandHandler<RemoveMangaHistoryCommand, Deleted>
{
    private readonly IUnitOfWork _unitOfWork;
    public RemoveMangaHistoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Deleted>> Handle(RemoveMangaHistoryCommand request, CancellationToken cancellationToken)
    {
        var mangaHistory = await _unitOfWork.HistoryRepository.FindAll(x=> x.UserId == request.UserId && x.MangaId == request.MangaId).ToListAsync(cancellationToken) ;
        _unitOfWork.HistoryRepository.RemoveMultiple(mangaHistory);
        await _unitOfWork.SaveChangesAsync(cancellationToken) ;
        return ResultType.Deleted;
    }
}
