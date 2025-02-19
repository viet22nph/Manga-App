using MangaApp.Domain.Abstractions.Repositories;
using MangaApp.Domain.Entities;

namespace MangaApp.Application.Abstraction.Repositories;

public interface IPermissionRepository : IRepositoryBase<Permission, Guid>
{
    Task<bool> CheckPermissionForUserAsync(Guid userId, List<string> listPermissions);

}