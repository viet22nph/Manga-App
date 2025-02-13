

using MangaApp.Domain.Abstractions;
using MangaApp.Domain.Entities.Identity;

namespace MangaApp.Domain.Entities;

public class Permission: EntityBase<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public virtual ICollection<RolePermission>? RolePermissions { get; set; }
    public int ResourceId { get; set; } // Nhóm chức năng chứa quyền này
    public virtual Resource? Resource { get; set; }
}
