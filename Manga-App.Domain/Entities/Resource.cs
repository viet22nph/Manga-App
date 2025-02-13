

namespace MangaApp.Domain.Entities;

public class Resource : Abstractions.EntityBase<int>
{
    public string Name { get; set; }
    public string? Description { get; set; }

    public virtual ICollection<Permission>? Permissions { get; set; }
}
