
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
namespace MangaApp.Contract.Services.V1.Rating;

public static class Command
{ 
    public record CreateOrUpdateMangaRatingCommand(Guid UserId, Guid MangaId, int Rating): ICommand<Success>;

    public record DeleteMangaRatingCommand(Guid UserId, Guid MangaId) : ICommand<Deleted>;
}
