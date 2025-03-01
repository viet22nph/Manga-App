
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;

namespace MangaApp.Contract.Services.V1.History;

public static class Command
{
    public record AddReadingHistoryCommand(Guid UserId, Guid MangaId, Guid ChapterId): ICommand<Success>;
}
