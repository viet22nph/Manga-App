

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Rating.Command;

namespace MangaApp.Application.UserCases.V1.Commands.Rating;

public class DeleteMangaRatingCommandHandler : ICommandHandler<DeleteMangaRatingCommand, Deleted>
{

    private readonly IUnitOfWork _unitOfWork;
    public DeleteMangaRatingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public  async Task<Result<Deleted>> Handle(DeleteMangaRatingCommand request, CancellationToken cancellationToken)
    {
        var rating = await _unitOfWork.RatingRepository
            .FindAll(x=> x.UserId == request.UserId && x.MangaId == request.MangaId)
            .AsTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (rating is null) {
            return Error.Failure(code: nameof(Domain.Entities.Rating), description: "User has not rated this manga.");
        }
        _unitOfWork.RatingRepository.Remove(rating);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultType.Deleted;
    }
}
