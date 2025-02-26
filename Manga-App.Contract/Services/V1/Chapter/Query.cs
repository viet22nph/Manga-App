

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Contract.Shares;
using static MangaApp.Contract.Services.V1.Chapter.Response;

namespace MangaApp.Contract.Services.V1.Chapter;

public static class Query
{
    public record GetListChapterQuery(
        Guid? MangaId,
        string? SortColumn,
        SortOrder SortOrder,
        int PageIndex,
        int PageSize
        )
    : IQuery<Pagination<ChapterResponse>>;

    public record GetChapterByIdQuery(Guid ChapterId)
        : IQuery<ChapterDetailReponse>;
    public record GetChapterBySlugQuery(string Slug)
     : IQuery<ChapterDetailReponse>;
}
