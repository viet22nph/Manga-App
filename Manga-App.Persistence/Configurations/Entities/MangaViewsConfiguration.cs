

using MangaApp.Domain.Entities;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public class MangaViewsConfiguration: MappingEntityTypeConfiguration<MangaViews>
{

    public override void Configure(EntityTypeBuilder<MangaViews> builder)
    {
        builder.ToTable(TableNames.MangaViews);
        builder.HasKey(t => t.Id);

        builder.HasOne(e => e.Manga)
             .WithMany(m => m.Views)
                  .HasForeignKey(e => e.MangaId)
                  .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(mv => new { mv.MangaId, mv.ViewDate })
            .IsUnique();
        base.Configure(builder);
    }

}
