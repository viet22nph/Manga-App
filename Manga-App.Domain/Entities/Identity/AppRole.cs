
using MangaApp.Domain.Abstractions.Entities;
using Microsoft.AspNetCore.Identity;

namespace MangaApp.Domain.Entities.Identity;

public class AppRole : IdentityRole<Guid>, IAuditable
{
    public string? Description { get; set; }
    public virtual ICollection<IdentityRoleClaim<Guid>>? Claims { get; set; }
    public ICollection<AppUserRole>? UserRoles { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset ModifiedDate { get; set; }
}
