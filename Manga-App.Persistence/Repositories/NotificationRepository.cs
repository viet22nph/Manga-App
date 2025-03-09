

using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Abstractions.Repositories;
using MangaApp.Domain.Entities;

namespace MangaApp.Persistence.Repositories;

public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context)
    {
    }
}
