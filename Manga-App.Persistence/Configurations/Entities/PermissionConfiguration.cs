
using MangaApp.Domain.Entities;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public class PermissionConfiguration: MappingEntityTypeConfiguration<Permission>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(TableNames.Permission);
        builder.HasKey(x => x.Id);  
        builder.Property(x=> x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x=> x.Description).HasMaxLength(150).IsRequired(false);
        


        builder.HasOne(x=> x.Resource)
            .WithMany(y=> y.Permissions)
            .HasForeignKey(x=> x.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);
        base.Configure(builder);
    }
}
