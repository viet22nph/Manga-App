using MangaApp.Domain.Entities;
using MangaApp.Persistence.Configurations;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public sealed class CountryConfiguration : MappingEntityTypeConfiguration<Country>
{
    public override void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable(TableNames.Country);
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).IsRequired();
        builder.Property(c => c.Name).IsRequired();

        base.Configure(builder);
    }
}
