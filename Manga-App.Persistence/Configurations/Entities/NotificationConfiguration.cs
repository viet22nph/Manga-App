
using MangaApp.Domain.Entities;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MangaApp.Persistence.Configurations.Entities;

public class NotificationConfiguration: MappingEntityTypeConfiguration<Notification>
{

    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable(TableNames.Notification);
        builder.HasKey(t => t.Id);
        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(250);
        builder.Property(x=> x.Read)
            .IsRequired();
        builder.Property(x=> x.Url)
            .IsRequired(false)
            .HasMaxLength(50);
        builder.Property(x=> x.SenderId)
            .IsRequired(false);

        builder.HasOne(x => x.Receive)
            .WithMany(y => y.ReceiveNotifications)
            .HasForeignKey(x => x.ReceiveId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Sender)
            .WithMany(y => y.SenderNotifications)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.NoAction);
        base.Configure(builder);
    }
}
