
namespace MangaApp.Application.Abstraction.Services.CacheService;

public interface ILocker
{
    bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action);
}
