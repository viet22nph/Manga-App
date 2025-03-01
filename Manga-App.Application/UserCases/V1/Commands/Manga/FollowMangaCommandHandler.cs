

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangaApp.Domain.Entities;
using MangApp.Application.Abstraction;
using static MangaApp.Contract.Services.V1.Follow.Command;

namespace MangaApp.Application.UserCases.V1.Commands.Manga;

public class FollowMangaCommandHandler : ICommandHandler<FollowMangaCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;           
    public FollowMangaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<Success>> Handle(FollowMangaCommand request, CancellationToken cancellationToken)
    {
        var manga = await _unitOfWork.MangaRepository.FindByIdAsync(request.MangaId);
        if (manga is null) { 
            return Error.NotFound(code: nameof(Domain.Entities.Manga), description:"Manga not found");
        }
        var follow = await _unitOfWork.FollowRepository.FindSingleAsync(x=> x.UserId == request.UserId && x.MangaId == request.MangaId, true,   cancellationToken);

        if (follow is not null) {
            return Error.Failure(code: nameof(Domain.Entities.Follow), description: "Follow already exists");
        }
        var newFollow = new Follow
        (
            request.UserId,
            request.MangaId
        );
        _unitOfWork.FollowRepository.Add(newFollow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultType.Success;
    }
}
