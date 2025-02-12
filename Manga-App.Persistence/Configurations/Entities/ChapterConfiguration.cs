using MangaApp.Domain.Entities;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public sealed class ChapterConfiguration: MappingEntityTypeConfiguration<Chapter>
{
    public override void Configure(EntityTypeBuilder<Chapter> builder)
    {
        builder.ToTable(TableNames.Chapter);
        builder.HasKey(x => x.Id);
        builder.Property(x=> x.Title).IsRequired().HasMaxLength(50);
        builder.Property(x=> x.Number).IsRequired();
        builder.Property(x=> x.Slug).IsRequired().HasMaxLength(50);

        builder.HasOne(x=> x.ApprovedBy)
            .WithMany(y=>y.ChapterApproved)
            .HasForeignKey(x=>x.ApprovedById);
        builder.HasOne(x => x.Manga)
            .WithMany(y => y.Chapters)
            .HasForeignKey(x => x.MangaId)
            .OnDelete(DeleteBehavior.Cascade);
        base.Configure(builder);
    }
}
