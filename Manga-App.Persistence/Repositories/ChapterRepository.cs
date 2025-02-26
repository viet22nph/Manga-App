
using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Entities;

namespace MangaApp.Persistence.Repositories;

internal class ChapterRepository : RepositoryBase<Chapter, Guid>, IChapterRepository
{
    public ChapterRepository(AppDbContext context) : base(context)
    {
    }
}
