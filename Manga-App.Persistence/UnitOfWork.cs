

using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Persistence.Repositories;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace MangaApp.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    public DbContext GetDbContext()
    {
        return _context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IMangaRepository _mangaRepository;
    public IMangaRepository MangaRepository
    {
        get
        {
            if (_mangaRepository == null)
            {
                _mangaRepository = new MangaRepository(_context);
            }
            return _mangaRepository;
        }
    }

}
