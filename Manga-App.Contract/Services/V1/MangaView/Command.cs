
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;

namespace MangaApp.Contract.Services.V1.MangaView;

public static class Command
{
    public record IncreaseMangaViewCommand(Guid MangaId) :ICommand<Success>;
}
