

using MangaApp.Domain.Entities.Identity;
using System.Data;

namespace MangaApp.Domain.Entities;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public AppRole? Role { get; set; }

    public Guid PermissionId { get; set; }
    public Permission? Permission { get; set; }
}
