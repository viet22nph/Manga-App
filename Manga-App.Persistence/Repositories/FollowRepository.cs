
using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Entities;

namespace MangaApp.Persistence.Repositories;

public class FollowRepository : RepositoryBase<Follow>, IFollowRepository
{
    public FollowRepository(AppDbContext context) : base(context)
    {
    }
}
