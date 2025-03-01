using MangaApp.Domain.Entities;
using MangaApp.Persistence.Configurations;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public class FollowConfiguration : MappingEntityTypeConfiguration<Follow>
{
    public override void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable(TableNames.Follow);
        builder.HasKey(x => new { x.UserId, x.MangaId });

        builder.HasOne(x => x.User)
            .WithMany(y => y.Follows)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Manga)
            .WithMany(y => y.Follows)
            .HasForeignKey(x => x.MangaId)
            .OnDelete(DeleteBehavior.Cascade);
        base.Configure(builder);
    }
}
