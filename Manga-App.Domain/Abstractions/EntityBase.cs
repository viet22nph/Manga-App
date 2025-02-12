
using MangaApp.Domain.Abstractions.Entities;

namespace MangaApp.Domain.Abstractions;
public abstract class EntityBase<TKey> : IEntityBase<TKey>
{
    public TKey Id { get; set; }
}