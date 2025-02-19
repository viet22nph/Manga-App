

using MangaApp.Domain.Abstractions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MangaApp.Persistence.Interceptors;

public class EntityAuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EntityAuditInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
          DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken)
    {
        var context = eventData.Context;
        if (context == null) return result;

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;
        if (string.IsNullOrEmpty(userId)) return result;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditable auditableEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    auditableEntity.CreatedBy = Guid.Parse(userId);
                    auditableEntity.CreatedDate = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    auditableEntity.ModifiedBy = Guid.Parse(userId);
                    auditableEntity.ModifiedDate = DateTimeOffset.UtcNow;
                }
            }
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
