

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using static MangaApp.Contract.Services.V1.Manga.Response;

namespace MangaApp.Contract.Services.V1.History;

public static class Query
{
    public record GetHistoryQuery(Guid UserId, int PageIndex, int PageSize): IQuery<Pagination<MangaResponse>>;
}
