using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using static MangaApp.Contract.Services.V1.Manga.Command;

namespace MangaApp.Application.UserCases.V1.Commands.Manga;

public sealed class RejectMangaCommandHandler : ICommandHandler<RejectMangaCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    public RejectMangaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Success>> Handle(RejectMangaCommand request, CancellationToken cancellationToken)
    {
        var manga = await _unitOfWork.MangaRepository.FindByIdAsync(request.MangaId, cancellationToken);
        if (manga == null)
        {
            return Error.Failure("Not found manga");
        }
        var result = manga.Reject(request.UserId);
        if (result.IsError)
        {
            return result;
        }
        _unitOfWork.MangaRepository.Update(manga);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
}
