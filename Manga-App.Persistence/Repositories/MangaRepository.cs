
using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Entities;

namespace MangaApp.Persistence.Repositories;

public class MangaRepository : RepositoryBase<Manga, Guid>, IMangaRepository
{
    public MangaRepository(AppDbContext context) : base(context)
    {
    }

}
