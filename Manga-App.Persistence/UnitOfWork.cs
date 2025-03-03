

using Manga_App.Persistence.Repositories;
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
    private IChapterRepository _chapterRepository;
    public IChapterRepository ChapterRepository
    {
        get
        {
            if (_chapterRepository == null)
            {
                _chapterRepository = new ChapterRepository(_context);
            }
            return _chapterRepository;
        }
    }

    private IHistoryRepository _historyRepository;
    public IHistoryRepository HistoryRepository
    {
        get
        {
            if ((_historyRepository == null))
            {
                _historyRepository = new HistoryRepository(_context);
            }
            return _historyRepository;
        }
    }
    private IFollowRepository _followRepository;
    public IFollowRepository FollowRepository
    {
        get
        {
            if (_followRepository == null)
            {
                _followRepository = new FollowRepository(_context);
            }
            return _followRepository;
        }
    }
    private IRatingRepository _ratingRepository;
    public IRatingRepository RatingRepository
    {
        get
        {
            if (_ratingRepository == null)
            {
                _ratingRepository = new RatingRepository(_context);
            }
            return _ratingRepository;
        }
    }

    private IViewRepository _viewRepository;
    public IViewRepository ViewRepository
    {
        get
        {
            if(_viewRepository == null)
            {
                _viewRepository = new ViewRepository(_context);
            }    
            return _viewRepository;
        }
    }
}
