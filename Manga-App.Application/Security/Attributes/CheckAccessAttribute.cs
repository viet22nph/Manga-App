using MangaApp.Application.Abstraction.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Web.Mvc;

namespace MangaApp.Application.Security.Attributes;

[AttributeUsage(AttributeTargets.Class |
                         AttributeTargets.Method
                       , AllowMultiple = true
                       , Inherited = true)]

public class CheckAccessAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
{
    private string[] _permission;
    public CheckAccessAttribute(params string[] permission)
    {
        _permission = permission;
    }


    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user is null || !user.Identity!.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        var permissionRepo = context.HttpContext.RequestServices.GetService(typeof(IPermissionRepository)) as IPermissionRepository;
        if (permissionRepo == null)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }
        var hasPermission = await permissionRepo.CheckPermissionForUserAsync(userId, _permission.ToList());
        if (!hasPermission)
        {
            context.Result = new ForbidResult();
            return;
        }
        return;
    }
}
