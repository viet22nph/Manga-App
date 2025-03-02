

namespace MangaApp.Contract.Services.V1.Rating;

public static class Response
{
    public record RatingResponse(int Rating, DateTimeOffset CreatedAt);
}
