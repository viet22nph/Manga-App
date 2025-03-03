

using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Entities;

namespace MangaApp.Persistence.Repositories;

public class ViewRepository : RepositoryBase<MangaViews, int>, IViewRepository
{
    public ViewRepository(AppDbContext context) : base(context)
    {
    }
}
