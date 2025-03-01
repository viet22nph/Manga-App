

using MangaApp.Domain.Abstractions;
using MangaApp.Domain.Entities.Identity;
using MassTransit.Logging;

namespace MangaApp.Domain.Entities;

public class History: EntityBase<long>
{
    public Guid MangaId { get; set; }
    public Guid UserId { get; set; }
    public Guid ChapterId { get; set; }
    public DateTimeOffset LastReadAt { get; set; }

    public virtual Manga Manga { get; set; }
    public virtual AppUser User { get; set; }
    public virtual Chapter Chapter { get; set; }

    public History( Guid userId, Guid mangaId, Guid chapterId)
    {
        MangaId = mangaId;
        UserId = userId;
        ChapterId = chapterId;
        LastReadAt = DateTimeOffset.UtcNow;
    }
}
