
using MangaApp.Domain.Abstractions.Entities;
using MangaApp.Domain.Entities.Identity;
using MangaApp.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using MangaApp.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace MangaApp.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid, Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>, AppUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    private readonly IHttpContextAccessor _contextAccessor;
    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor contextAccessor) : base(options)
    {
        _contextAccessor = contextAccessor;
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {

        var softDeleteEntites = typeof(ISoftDelete).Assembly.GetTypes()
            .Where(t => typeof(ISoftDelete).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
        foreach (var softDeleteEntity in softDeleteEntites)
        {
            builder.Entity(softDeleteEntity)
                .HasQueryFilter(generateQueryFilterLambda(softDeleteEntity));
            builder.Entity(softDeleteEntity).HasIndex("IsDeleted")
            .HasFilter("[IsDeleted] = 0");
        }
        // map configuration entity
        var typeConfigurations = Assembly.GetExecutingAssembly().GetTypes().Where(type =>
            (type.BaseType?.IsGenericType ?? false) &&
            (type.BaseType.GetGenericTypeDefinition() == typeof(MappingEntityTypeConfiguration<>))
        );
        foreach (var item in typeConfigurations)
        {
            var configuration = (IMappingConfiguration)Activator.CreateInstance(item);
            configuration.ApplyConfiguration(builder);
        }
    }
    private LambdaExpression? generateQueryFilterLambda(Type type)
    {
        var parameter = Expression.Parameter(type, "w");
        var falseContantValue = Expression.Constant(false);
        var propertyAccess = Expression.PropertyOrField(parameter, nameof(ISoftDelete.IsDeleted));
        var equalExpression = Expression.Equal(propertyAccess, falseContantValue);
        var lambda = Expression.Lambda(equalExpression, parameter);
        return lambda;
    }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppUserRole> AppUserRoles { get; set; }
    public DbSet<Manga> Mangas { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<MangaGenre> MangaGenres { get; set;}
    public DbSet<Chapter> Chapters { get; set; }
    public DbSet<ChapterImage> ChapterImages { get; set; }
    public DbSet<Country> Countries { get; set; }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Rating > Rating { get; set; }
    public DbSet<History> Histories { get; set; }
    public DbSet<Follow> Follows { get; set; }
}