using MangaApp.Domain.Entities;
using MangaApp.Persistence.Configurations;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public sealed class GenreConfiguration: MappingEntityTypeConfiguration<Genre>
{
    public override void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable(TableNames.Genre);
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.Slug).IsRequired().HasMaxLength(65);
        builder.HasIndex(x => new { x.Slug, x.Name }).IsUnique();
        base.Configure(builder);
    }

}
