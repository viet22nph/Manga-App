
using MangaApp.Application.Abstraction.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MangApp.Application.Abstraction;

public interface IUnitOfWork : IAsyncDisposable
{
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    DbContext GetDbContext();

    IMangaRepository MangaRepository { get; }
    IChapterRepository ChapterRepository {  get; }
    IHistoryRepository HistoryRepository { get; }
    IFollowRepository FollowRepository { get; }
    IRatingRepository RatingRepository { get; }
}