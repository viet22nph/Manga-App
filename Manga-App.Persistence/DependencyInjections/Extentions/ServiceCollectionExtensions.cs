using MangaApp.Application.Abstraction.Repositories;
using MangaApp.Domain.Abstractions.Repositories;
using MangaApp.Domain.Entities.Identity;
using MangaApp.Persistence.DependencyInjections.Options;
using MangaApp.Persistence.Interceptors;
using MangaApp.Persistence.Repositories;
using MangApp.Application.Abstraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MangaApp.Persistence.DependencyInjections.Extentions;

public static class ServiceCollectionExtensions
{
    public static void AddSqlServer(this IServiceCollection services)
    {
        services.AddDbContextPool<DbContext, AppDbContext>((provider, builder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<SqlServerRetryOptions>>();
            var options = optionsMonitor.CurrentValue;
            
            builder.EnableDetailedErrors(true)
                .EnableSensitiveDataLogging(true)
                .UseLazyLoadingProxies(false)
                .UseSqlServer(
                    connectionString: configuration.GetConnectionString("Connection"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.ExecutionStrategy(dependencies =>
                            new SqlServerRetryingExecutionStrategy(
                                dependencies: dependencies,
                                maxRetryCount: options.MaxRetryCount,
                                maxRetryDelay: options.MaxRetryDelay,
                                errorNumbersToAdd: options.ErrorNumbersToAdd)
                        );
                        sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                    })
                .AddInterceptors();
        });

        services.AddDbContext<AppDbContext>(ServiceLifetime.Transient);
    }
    public static void AddIdentity(this IServiceCollection services)
    {
        // add indentity
        services.AddIdentityCore<AppUser>(options =>
        {
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.AllowedForNewUsers = false;
            options.Lockout.MaxFailedAccessAttempts = 5;

        })
        .AddRoles<AppRole>()
        .AddEntityFrameworkStores<AppDbContext>();
        // config pass
        services.Configure<IdentityOptions>(options =>
        {
            // Default Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

        });

    }
    public static OptionsBuilder<SqlServerRetryOptions> ConfigureSqlServerRetryOption(this IServiceCollection services, IConfiguration configuration)

         => services.AddOptions<SqlServerRetryOptions>()
           .Bind(configuration.GetSection("SqlServerRetryOptions"))
           .ValidateDataAnnotations()
           .ValidateOnStart();

    public static void AddServicePersitence(this IServiceCollection services)
    {

        services.AddTransient<IUnitOfWork, UnitOfWork>(); // Thay vì AddTransient nếu cần
        services.AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
        services.AddTransient<IPermissionRepository, PermissionRepository>();

    }
}

