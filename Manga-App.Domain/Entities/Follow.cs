

using MangaApp.Domain.Abstractions;
using MangaApp.Domain.Entities.Identity;
using MassTransit.Caching;

namespace MangaApp.Domain.Entities;

public class Follow
{
    public Guid UserId { get; set; }
    public Guid MangaId { get; set; }
    public DateTimeOffset FollowedAt { get; set; } = DateTimeOffset.UtcNow;

    public virtual AppUser User { get; set; }
    public virtual Manga Manga { get; set; }


    public Follow(Guid userId, Guid mangaId)
    {
        UserId = userId;
        MangaId = mangaId;
        FollowedAt = DateTimeOffset.UtcNow;
    }

}
