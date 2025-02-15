using MangaApp.Application.Abstraction.Services;
using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Infrastructure.Authentication;
using MangaApp.Infrastructure.DependencyInjection.Options;
using MangaApp.Infrastructure.RedisCache;
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
}