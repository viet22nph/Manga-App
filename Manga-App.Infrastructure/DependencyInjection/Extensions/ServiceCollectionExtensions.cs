using Amazon;
using Amazon.S3;
using Manga_App.Infrastructure.DependencyInjection.Options;
using MangaApp.Application.Abstraction.Services;
using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Infrastructure.Authentication;
using MangaApp.Infrastructure.Aws;
using MangaApp.Infrastructure.DependencyInjection.Options;
using MangaApp.Infrastructure.MessageQueue.Consumer;
using MangaApp.Infrastructure.RedisCache;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public static class ServiceCollectionExtensions
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        services
           .AddAuthentication(options =>
           {
               options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
           })
           .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
           {
               JwtOptions jwtOptions = new JwtOptions();
               configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);
               var key = System.Text.Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
               // storing the JWT in the AuthenticationProperties allows you to retrieve it from elsewhere withinyour application
               // example var  accesstoken = await HttpContext.GetTokenAsync(access_token);
               o.SaveToken = true;
               o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
               {
                   ValidateAudience = false, // on production is true
                   ValidateIssuer = false, // on production is true
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = jwtOptions.Issuer,
                   ValidAudience = jwtOptions.Audience,
                   IssuerSigningKey = new SymmetricSecurityKey(key),
                   ClockSkew = TimeSpan.Zero,
               };
               o.Events = new JwtBearerEvents
               {
                   OnAuthenticationFailed = context =>
                   {
                       if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                       {
                           context.Response.Headers.Append("IS-TOKEN-EXPRIED", "true");
                       }
                       return Task.CompletedTask;
                   }
               };
           });
        services.AddAuthorization();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
    }
    public static void AddCacheRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));

        services.AddScoped<IRedisConnectionWrapper, RedisConnectionWrapper>();
        services.AddScoped<ICacheManager, RedisCacheManager>();
    }
    public static void AddAws(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AwsS3Options>(configuration.GetSection(nameof(AwsS3Options)));
        var awsOptions = configuration.GetSection(nameof(AwsS3Options));
        services.AddSingleton<IAmazonS3>(sp =>
            new AmazonS3Client(
                awsOptions["AccessKey"],
                awsOptions["SecretKey"],
                RegionEndpoint.GetBySystemName(awsOptions["Region"])
            ));

        services.AddScoped<IAwsS3Service, AwsS3Service>();
    }

    // config masstrasit
    public static IServiceCollection AddMasstransitConfigurationRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {

        var masstransitConfiguration = new MasstransitConfiguration();
            configuration.GetSection(nameof(MasstransitConfiguration)).Bind(masstransitConfiguration);
        services.AddMassTransit(config =>
        {
            config.AddConsumer<GetChapterRequestConsumer>();
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(masstransitConfiguration.Host, masstransitConfiguration.VHost, h =>
                {
                    h.Username(masstransitConfiguration.UserName);
                    h.Password(masstransitConfiguration.Password);
                });
                cfg.ReceiveEndpoint("new-chapter-queue", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    // notification exchange
                    e.Bind(masstransitConfiguration.ExchangeName, x =>
                    {
                        x.ExchangeType = masstransitConfiguration.ExchangeType;
                        x.RoutingKey = "notification.new_chapter";
                    });
                    e.ConfigureConsumer<GetChapterRequestConsumer>(context);
                });

            });

        });
        return services;
    }
}