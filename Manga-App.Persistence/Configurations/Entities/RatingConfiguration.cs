

using MangaApp.Domain.Entities;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public class RatingConfiguration: MappingEntityTypeConfiguration<Rating>
{
    public override void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.ToTable(TableNames.Rating);
        builder.HasKey(x=> new {x.UserId,x.MangaId});
        builder.Property(x => x.Score).IsRequired();
        builder.HasOne(x=> x.User)
            .WithMany(y=> y.Ratings)
            .HasForeignKey(x=>x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x=> x.Manga)
            .WithMany(y=> y.Ratings)
            .HasForeignKey(x=> x.MangaId)
            .OnDelete(DeleteBehavior.Cascade);
        base.Configure(builder);
    }
}
