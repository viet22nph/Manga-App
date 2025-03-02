

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Rating.Command;

namespace MangaApp.Application.UserCases.V1.Commands.Rating;

public class CreateOrUpdateMangaRatingCommandHandler : ICommandHandler<CreateOrUpdateMangaRatingCommand, Success>
{

    private readonly IUnitOfWork _unitOfWork;
    public CreateOrUpdateMangaRatingCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success>> Handle(CreateOrUpdateMangaRatingCommand request, CancellationToken cancellationToken)
    {
        var manga = await _unitOfWork.MangaRepository.FindByIdAsync(request.MangaId);
        if(manga is null)
        {
            return Error.NotFound(code: nameof(Domain.Entities.Manga), description: "Manga not found");
        }
        var rating = await _unitOfWork.RatingRepository
            .FindAll(x => x.UserId == request.UserId && x.MangaId == request.MangaId)
            .AsTracking()
            .FirstOrDefaultAsync(cancellationToken);
        if (rating is not null) {
            rating.Score = request.Rating;
            _unitOfWork.RatingRepository.Update(rating);
        }
        else
        {
            var newRating = new Domain.Entities.Rating
            {
                UserId = request.UserId,
                MangaId = request.MangaId,  
                Score = request.Rating,
            };
            _unitOfWork.RatingRepository.Add(newRating);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultType.Success;
    }
}
