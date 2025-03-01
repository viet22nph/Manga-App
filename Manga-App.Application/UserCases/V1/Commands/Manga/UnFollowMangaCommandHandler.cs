
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangaApp.Domain.Entities;
using MangApp.Application.Abstraction;
using static MangaApp.Contract.Services.V1.Follow.Command;

namespace MangaApp.Application.UserCases.V1.Commands.Manga;

public class UnFollowMangaCommandHandler : ICommandHandler<UnFollowMangaCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    public UnFollowMangaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<Success>> Handle(UnFollowMangaCommand request, CancellationToken cancellationToken)
    {
        var follow = await _unitOfWork.FollowRepository.FindSingleAsync(x => x.UserId == request.UserId && x.MangaId == request.MangaId, true,cancellationToken);

        if (follow is null)
        {
            return Error.Failure(code: nameof(Domain.Entities.Follow), description: "Follow does not exist");
        }
            
        _unitOfWork.FollowRepository.Remove(follow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultType.Success;
    }
}
