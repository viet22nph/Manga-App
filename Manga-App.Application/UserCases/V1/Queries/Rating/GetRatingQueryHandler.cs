using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using static MangaApp.Contract.Services.V1.Rating.Query;
using static MangaApp.Contract.Services.V1.Rating.Response;

namespace MangaApp.Application.UserCases.V1.Queries.Rating;

public class GetRatingQueryHandler : IQueryHandler<GetRatingQuery, RatingResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetRatingQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<RatingResponse>> Handle(GetRatingQuery request, CancellationToken cancellationToken)
    {
        var rating = await _unitOfWork.RatingRepository
            .FindSingleAsync(x=> x.UserId == request.UserId&& x.MangaId == request.MangaId);
        if (rating is null)
        {
            return Error.NotFound(code: nameof(Domain.Entities.Rating), description: "User has not rated this manga yet.");
        }

        var response = new RatingResponse(Rating: rating.Score, CreatedAt: rating.Created);
        return response;
    }
}
