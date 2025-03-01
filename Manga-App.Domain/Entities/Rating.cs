
using MangaApp.Domain.Entities.Identity;

namespace MangaApp.Domain.Entities;

public class Rating
{ 
    public Guid UserId { get; set; }
    public Guid MangaId { get; set; }
    public int Score {  get; set; }
    public DateTimeOffset Created {  get; set; }


    public virtual AppUser User { get; set; }
    public virtual Manga Manga { get; set; }

}
