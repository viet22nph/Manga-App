using MangaApp.Domain.Entities.Identity;
using MangaApp.Persistence.Configurations;
using MangaApp.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MangaApp.Persistence.Configurations.Entities.Identity;

public sealed class AppUserConfiguration : MappingEntityTypeConfiguration<AppUser>
{
    public override void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable(TableNames.AppUser);
        builder.HasKey(t => t.Id);
        builder.Property(x => x.UserName).HasMaxLength(50);
        builder.Property(x => x.NormalizedUserName).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(255);
        builder.Property(x => x.NormalizedEmail).HasMaxLength(255);
        builder.Property(x => x.FirstName).HasMaxLength(50).IsRequired(false);
        builder.Property(x => x.LastName).HasMaxLength(50).IsRequired(false);
        builder.Property(x => x.FullName).HasMaxLength(50).IsRequired(false);
        builder.Property(x => x.DisplayName).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Avatar).HasMaxLength(300).IsRequired(false);
        builder.Property(x => x.Gender).HasConversion<string>().HasMaxLength(10);
        builder.Property(x => x.CreatedDate);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.Property(x => x.PhoneNumber).HasMaxLength(15);

        builder.HasMany(x => x.Claims)
            .WithOne()
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();


        builder.HasMany(x => x.Logins)
            .WithOne()
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();

        builder.HasMany(x => x.UserTokens)
            .WithOne()
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();
        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(uc => uc.UserId)
            .IsRequired();

    }
}
