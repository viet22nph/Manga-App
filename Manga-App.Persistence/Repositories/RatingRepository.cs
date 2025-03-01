using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Entities;

namespace MangaApp.Persistence.Repositories;

public class RatingRepository : RepositoryBase<Rating>, IRatingRepository
{
    public RatingRepository(AppDbContext context) : base(context)
    {
    }
}
