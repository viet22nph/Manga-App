using MangaApp.Domain.Entities;
using MangaApp.Persistence.Configurations;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public  sealed class ChapterImageConfiguration: MappingEntityTypeConfiguration<ChapterImage>
{
    public override void Configure(EntityTypeBuilder<ChapterImage> builder)
    {
        builder.ToTable(TableNames.ChapterImage);
        builder.HasKey(x => x.Id);
        builder.Property(x=> x.Path).IsRequired().HasMaxLength(255);
        builder.Property(x => x.OrderIndex).IsRequired();
        builder.HasOne(x => x.Chapter)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.ChapterId)
                .OnDelete(DeleteBehavior.Cascade);


        base.Configure(builder);
    }
}
