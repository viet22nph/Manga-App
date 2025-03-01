
using MangaApp.Domain.Entities;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public class HistoryConfiguration: MappingEntityTypeConfiguration<History>
{
    public override void Configure(EntityTypeBuilder<History> builder)
    {
        builder.ToTable(TableNames.History);
        builder.HasKey(x=> x.Id);

        builder.HasOne(x => x.User)
            .WithMany(y => y.Histories)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Manga)
            .WithMany(y => y.Histories)
            .HasForeignKey(x => x.MangaId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Chapter)
            .WithMany(y => y.Histories)
            .HasForeignKey(x => x.ChapterId);

        base.Configure(builder);
    }
}
