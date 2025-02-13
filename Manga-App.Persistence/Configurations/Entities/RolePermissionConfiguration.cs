

using MangaApp.Domain.Entities;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public class RolePermissionConfiguration : MappingEntityTypeConfiguration<RolePermission>
{
    public override void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(TableNames.RolePermission);
        builder.HasKey(x=> new {x.RoleId, x.PermissionId});


        builder.HasOne(x=> x.Permission)
            .WithMany(y=> y.RolePermissions)
            .HasForeignKey(x=> x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasOne(x=> x.Role)
            .WithMany(y=> y.RolePermissions)
            .HasForeignKey(x=> x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        base.Configure(builder);

    }
}
