
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Enums;
using static MangaApp.Contract.Services.V1.Manga.Response;

namespace MangaApp.Contract.Services.V1.Manga;

public static class Query
{
    public record GetListMangaQuery(
     string? GenreSlug,
     MangaStatus? Status,
     string? SearchTerm,
     string? SortColumn,
     SortOrder SortOrder,
     ApprovalStatus? Approval,
     bool? IsPublished,
     int PageIndex,
     int PageSize)
     : IQuery<Pagination<MangaResponse>>;

    public record GetPublicMangaQuery(
      string? GenreSlug,
      MangaStatus? Status,
      string? SearchTerm,
      string? SortColumn,
      SortOrder SortOrder,
      int PageIndex,
      int PageSize)
      : IQuery<Pagination<MangaResponse>>;

    public record GetMangaByIdQuery(Guid Id): IQuery<MangaResponse>;

    public record GetMangaBySlugQuery(string Slug) : IQuery<MangaResponse>;
}
