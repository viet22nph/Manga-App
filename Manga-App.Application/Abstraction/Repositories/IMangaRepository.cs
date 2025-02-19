
using MangaApp.Domain.Abstractions.Repositories;
using MangaApp.Domain.Entities;

namespace MangaApp.Application.Abstraction.Repositories;

public interface IMangaRepository : IRepositoryBase<Manga, Guid>
{
}
