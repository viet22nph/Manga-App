
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using static MangaApp.Contract.Services.V1.Manga.Response;

namespace MangaApp.Contract.Services.V1.Follow;

public static class Query
{
    public record GetFollowedMangaQuery(Guid UserId, int PageIndex, int PageSize) : IQuery<Pagination<MangaResponse>>;
}
