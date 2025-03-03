
using MangaApp.Domain.Abstractions;

namespace MangaApp.Domain.Entities;

public class MangaViews: EntityBase<int>
{
   
    public Guid MangaId { get; set; }
    public uint ViewCount {  get; set; }
    public DateTime ViewDate {  get; set; } 
    public virtual Manga Manga { get; set; }
    

}
