

using MangaApp.Domain.Abstractions.Repositories;
using MangaApp.Domain.Abstractions;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MangaApp.Persistence.Repositories;

public class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>, IDisposable
        where TEntity : EntityBase<TKey>
{

    protected readonly AppDbContext _context;

    public RepositoryBase(AppDbContext context)
        => _context = context;

    public void Dispose()
        => _context?.Dispose();

    public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> items = _context.Set<TEntity>().AsNoTracking(); // Importance Always include AsNoTracking for Query Side
        if (includeProperties != null)
        {
            items = items.AsSplitQuery();
            foreach (var includeProperty in includeProperties)
                items = items.Include(includeProperty);

        }
        if (predicate is not null)
            items = items.Where(predicate);

        return items;
    }

    public async Task<TEntity?> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties).AsTracking().SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    public async Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties)
        => await FindAll(null, includeProperties).AsTracking().SingleOrDefaultAsync(predicate, cancellationToken);

    public void Add(TEntity entity)
        => _context.Add(entity);

    public void Remove(TEntity entity)
        => _context.Set<TEntity>().Remove(entity);

    public void RemoveMultiple(List<TEntity> entities)
        => _context.Set<TEntity>().RemoveRange(entities);

    public void Update(TEntity entity)
        => _context.Set<TEntity>().Update(entity);
    public IQueryable<TEntity> FromSqlRaw(string sql, params object[] parameters)
    {
        return _context.Set<TEntity>().FromSqlRaw(sql, parameters);
    }
}
