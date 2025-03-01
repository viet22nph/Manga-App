

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;

namespace MangaApp.Contract.Services.V1.Follow;

public static class Command
{
    public record FollowMangaCommand(Guid UserId, Guid MangaId) : ICommand<Success>;
    public record UnFollowMangaCommand(Guid UserId, Guid MangaId) : ICommand<Success>;
}
