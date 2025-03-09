

using MangaApp.Contract.Shares.Enums;
using MangaApp.Domain.Abstractions;
using MangaApp.Domain.Abstractions.Entities;
using MangaApp.Domain.Entities.Identity;
namespace MangaApp.Domain.Entities;

public class Notification: EntityBase<Guid>, ISoftDelete
{
    public Guid ReceiveId { get; set; }
    public virtual AppUser Receive { get; set; }

    public Guid? SenderId { get; set; }
    public virtual AppUser? Sender { get; set; }
    public NotificationType Type {  get; set; }
    public string Message { get; set; }
    public string Url { get; set; }
    public bool Read { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
}
