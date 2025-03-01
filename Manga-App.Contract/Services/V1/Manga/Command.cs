
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Contract.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MangaApp.Contract.Services.V1.Manga;

public static class Command
{
    public record CreateMangaCommand(
        string Title,
        string? AnotherTitle,
        string? Description,
        string? Author,
        IFormFile Thumbnail,
        int CountryId,
        MangaStatus Status,
        ContentRating ContentRating,
        List<int>? GenresId,
        string? Slug,
        int? Year
    ) : ICommand<Success>;

    public record ApproveMangaCommand(
        Guid MangaId,
        Guid UserId): ICommand<Success>;

    public record RejectMangaCommand(
        Guid MangaId,
        Guid UserId) : ICommand<Success>;

    public record PublishMangaCommand(Guid MangaId): ICommand<Success>;

    public record UnPublishMangaCommand(Guid MangaId) : ICommand<Success>;

    public record UpdateMangaCommand(
       [FromRoute]
       Guid Id,
       string Title,
       string? AnotherTitle,
       string? Description,
       string? Author,
       IFormFile? Thumbnail,
       int CountryId,
       MangaStatus Status,
       ContentRating ContentRating,
       List<int> GenresId,
       string? Slug,
       int? Year
    ) : ICommand<Success>;

    public record FollowMangaCommand(Guid UserId, Guid MangaId): ICommand<Success>;
    public record UnFollowMangaCommand(Guid UserId, Guid MangaId) : ICommand<Success>;
}
