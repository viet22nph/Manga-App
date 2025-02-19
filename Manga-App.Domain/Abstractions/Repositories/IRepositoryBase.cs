﻿
using System.Linq.Expressions;

namespace MangaApp.Domain.Abstractions.Repositories;

public interface IRepositoryBase<TEntity, in Tkey> where TEntity : class
{
    Task<TEntity?> FindByIdAsync(Tkey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

    Task<TEntity?> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperties);

    IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null, params Expression<Func<TEntity, object>>[] includeProperties);

    void Add(TEntity entity);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    void RemoveMultiple(List<TEntity> entities);
    IQueryable<TEntity> FromSqlRaw(string sql, params object[] parameters);
}   
