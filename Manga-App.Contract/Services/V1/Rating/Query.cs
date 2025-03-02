

using MangaApp.Contract.Abstractions.Messages;
using static MangaApp.Contract.Services.V1.Rating.Response;

namespace MangaApp.Contract.Services.V1.Rating;

public static class Query
{
    public record GetRatingQuery(Guid UserId, Guid MangaId): IQuery<RatingResponse>;
}
