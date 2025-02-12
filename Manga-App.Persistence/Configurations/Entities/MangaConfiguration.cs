using MangaApp.Contract.Shares.Enums;
using MangaApp.Domain.Entities;
using MangaApp.Persistence.Configurations;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public sealed class MangaConfiguration: MappingEntityTypeConfiguration<Manga>
{

    public override void Configure(EntityTypeBuilder<Manga> builder)
    {
        builder.ToTable(TableNames.Manga);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AnotherTitle).IsRequired(false).HasMaxLength(100);
        builder.Property(x => x.Author).HasMaxLength(100).IsRequired(false);
        builder.Property(x => x.Thumbnail).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(x => x.ContentRating).HasConversion<string>().HasDefaultValue(ContentRating.Safe).HasMaxLength(10);
        builder.Property(x=> x.Status).HasConversion<string>().HasMaxLength(10);
        builder.Property(x=> x.ApprovalStatus).HasConversion<string>().HasMaxLength(15);
        builder.Property(x=> x.Description).IsRequired(false).HasMaxLength(1000);
        
        builder.HasOne(x=> x.ApprovedBy)
            .WithMany(y=>y.MangaApproved)
            .HasForeignKey(x=>x.ApprovedById);

        // FK Manga-> country
        builder.HasOne(x => x.Country).WithMany(y => y.Manga).HasForeignKey(x => x.CountryId);
        builder.HasIndex(x => x.Slug).IsUnique();
        base.Configure(builder);
    }
}
