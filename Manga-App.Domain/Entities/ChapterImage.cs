using MangaApp.Domain.Abstractions;

namespace MangaApp.Domain.Entities;

public class ChapterImage : EntityBase<int>
{
    public Guid ChapterId { get; set; }
    public string Path { get; set; }
    public int OrderIndex { get; set; }
    public DateTimeOffset Created { get; set; }
    public virtual Chapter Chapter { get; set; }
}