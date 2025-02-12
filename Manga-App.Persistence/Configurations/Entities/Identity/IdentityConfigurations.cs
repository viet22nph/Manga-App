
using MangaApp.Domain.Entities.Identity;
using MangaApp.Persistence.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace MangaApp.Persistence.Configurations.Entities.Identity;

public sealed class AppUserRoleConfiguration : MappingEntityTypeConfiguration<AppUserRole>
{

    public override void Configure(EntityTypeBuilder<AppUserRole> builder)
    {
        builder.ToTable(TableNames.AppUserRole);
        builder.HasKey(ur => new
        {
            ur.UserId,
            ur.RoleId
        });
    }
}


public sealed class AppRoleClaimConfiguration : MappingEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public override void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable(TableNames.AppRoleClaim);
        builder.HasKey(rc => rc.Id);
    }
}

public sealed class AppUserClaimConfiguration : MappingEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public override void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        builder.ToTable(TableNames.AppUserClaim);
        builder.HasKey(rc => rc.Id);
    }
}

public sealed class AppUserLoginConfiguration : MappingEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public override void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable(TableNames.AppUserLogin);
        builder.HasKey(rc => new
        {
            rc.LoginProvider,
            rc.ProviderKey
        });
    }
}

public sealed class AppUserTokenConfiguration : MappingEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public override void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        builder.ToTable(TableNames.AppUserToken);
        builder.HasKey(rc => new
        {
            rc.UserId,
            rc.LoginProvider,
            rc.Name
        });
    }
}
