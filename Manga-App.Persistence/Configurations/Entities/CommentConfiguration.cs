using MangaApp.Domain.Entities;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public class CommentConfiguration: MappingEntityTypeConfiguration<Comment>
{
    public override void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable(TableNames.Comment);
        builder.HasKey(t => t.Id);
        builder.Property(x=> x.Content).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Type).HasConversion<string>().IsRequired();
        builder.Property(x=> x.Action).HasConversion<string>().IsRequired();
        builder.Property(x=> x.Left).IsRequired();
        builder.Property(x=> x.Right).IsRequired();


        builder.HasOne(x => x.User)
          .WithMany(x => x.Comments)
          .HasForeignKey(x => x.UserId)
          .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ParentComment)
            .WithMany(x => x.Replies)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.NoAction);
        base.Configure(builder);
    }
}
