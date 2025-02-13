
using MangaApp.Domain.Entities;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public class ResourceConfiguration: MappingEntityTypeConfiguration<Resource>
{
    public override void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable(TableNames.Resource);
        builder.HasKey(t => t.Id);
        builder.Property(x=> x.Name).HasMaxLength(50).IsRequired();
        builder.Property(x=> x.Description).HasMaxLength(150).IsRequired(false);
        base.Configure(builder);
    }
}
