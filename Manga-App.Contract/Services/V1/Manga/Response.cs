
using MangaApp.Contract.Dtos.Country;
using MangaApp.Contract.Dtos.Genre;

namespace MangaApp.Contract.Services.V1.Manga;

public static class Response
{
    public record MangaResponse(
    Guid Id,
    string Title,
    string? AnotherTitle,
    string? Description,
    string? Author,
    string Thumbnail,
    string Slug,
    string Status,
    int? Year,
    List<string> Genres,
    string Country,
    bool Nsfw,
    string ContentRating,
    string ApprovalStatus,
    bool IsPublished,
    DateTimeOffset CreateDate,
    DateTimeOffset? UpdateDate
    );
    public record MangaPublicResponse(
      Guid Id,
      string Title,
      string? AnotherTitle,
      string? Description,
      string? Author,
      string Thumbnail,
      string Slug,
      string Status,
      int? Year,
      List<string> Genres,
      string Country,
      bool Nsfw,
      string ContentRating,
      DateTimeOffset CreateDate,
      DateTimeOffset? UpdateDate
      );

    public record MangaDetailResponse(
      Guid Id,
      string Title,
      string? AnotherTitle,
      string? Description,
      string? Author,
      string Thumbnail,
      string Slug,
      string Status,
      int? Year,
      string ContentRating,
      List<Genre> Genres,
      Country Country,
      DateTimeOffset CreateDate,
      DateTimeOffset? UpdateDate
    );
}
