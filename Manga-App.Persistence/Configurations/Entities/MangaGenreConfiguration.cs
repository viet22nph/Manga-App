using MangaApp.Domain.Entities;
using MangaApp.Persistence.Configurations;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public sealed class MangaGenreConfiguration: MappingEntityTypeConfiguration<MangaGenre>
{

    public override void Configure(EntityTypeBuilder<MangaGenre> builder)
    {

        builder.ToTable(TableNames.MangaGenre);
        builder.HasKey(x => new { x.MangaId, x.GenreId });
        builder.HasOne(x => x.Manga).WithMany(x => x.MangaGenres).HasForeignKey(x => x.MangaId);
        builder.HasOne(x => x.Genre).WithMany(x => x.MangaGenres).HasForeignKey(x => x.GenreId);
        base.Configure(builder);
    }
}
