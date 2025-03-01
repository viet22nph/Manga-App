
using System.Linq.Expressions;

namespace MangaApp.Domain.Abstractions.Repositories;

public interface IRepositoryBase<TEntity, in Tkey> where TEntity : class
{
    Task<TEntity?> FindByIdAsync(Tkey id, bool tracking = true, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, bool tracking = true, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

    IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, bool tracking = true, params Expression<Func<TEntity, object>>[] includeProperties);

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    void RemoveMultiple(List<TEntity> entities);
    IQueryable<TEntity> FromSqlRaw(string sql, params object[] parameters);
}
public interface IRepositoryBase<TEntity> where TEntity : class
{
    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, bool tracking = true, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

    IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, bool tracking = true, params Expression<Func<TEntity, object>>[] includeProperties);

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    void RemoveMultiple(List<TEntity> entities);
    /// <summary>
    /// Executes a raw SQL query and returns the result as an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <param name="sql">The raw SQL query to execute.</param>
    /// <param name="parameters">The parameters for the SQL query.</param>
    /// <returns>An <see cref="IQueryable{T}"/> of entities returned by the query.</returns>
    IQueryable<TEntity> FromSqlRaw(string sql, params object[] parameters);

}
