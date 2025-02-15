

namespace MangaApp.Infrastructure.DependencyInjection.Options;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
    public int ExpireAccess { get; set; }
    public int ExpireRefresh {  get; set; }
}
