

using MangaApp.Contract.Abstractions.Messages;
using static MangaApp.Contract.Services.V1.MangaView.Response;

namespace MangaApp.Contract.Services.V1.MangaView;

public static class Query
{
    public record GetViewMangaQuery(Guid MangaId): IQuery<ViewMangaResponse>;
}
