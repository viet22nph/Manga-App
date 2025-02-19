

using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaApp.Persistence.Repositories;

public class PermissionRepository : RepositoryBase<Permission, Guid>, IPermissionRepository
{
    public PermissionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> CheckPermissionForUserAsync(Guid userId, List<string> listPermissions)
    {
        var roleIds = await _context.AppUserRoles
              .Where(ur => ur.UserId == userId)
              .Select(ur => ur.RoleId)
              .ToListAsync();
        if (!roleIds.Any()) return false;

        var userPermissions = (await _context.RolePermissions
            .Where(rp => roleIds.Any(y => y == rp.RoleId))
            .Select(rp => rp.Permission!.Name)
            .ToListAsync());


        return userPermissions.Any(x => listPermissions.Any(y => x == y));
    }
}
