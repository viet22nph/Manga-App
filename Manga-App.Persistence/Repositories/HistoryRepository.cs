

using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Entities;
using MangaApp.Persistence;
using MangaApp.Persistence.Repositories;

namespace Manga_App.Persistence.Repositories;

public class HistoryRepository : RepositoryBase<History, long>, IHistoryRepository
{
    public HistoryRepository(AppDbContext context) : base(context)
    {
    }
}
