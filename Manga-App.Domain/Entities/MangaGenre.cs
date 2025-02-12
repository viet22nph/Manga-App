

namespace MangaApp.Domain.Entities;
public class MangaGenre
{
    public Guid MangaId { get; set; }
    public int GenreId { get; set; }

    public virtual Manga Manga { get; set; }
    public virtual Genre Genre { get; set; }
}

