

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using static MangaApp.Contract.Services.V1.Manga.Command;
namespace MangaApp.Application.UserCases.V1.Commands.Manga;

public sealed class ApproveMangaCommandHandler : ICommandHandler<ApproveMangaCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    public ApproveMangaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Success>> Handle(ApproveMangaCommand request, CancellationToken cancellationToken)
    {
        var manga = await _unitOfWork.MangaRepository.FindByIdAsync(request.MangaId);
        if (manga == null) {
            return Error.Failure("Not found manga");
        }
        var result = manga.Approve(request.UserId);
        if (result.IsError) {
            return result;
        }
        // Update lại manga
        _unitOfWork.MangaRepository.Update(manga);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
}
