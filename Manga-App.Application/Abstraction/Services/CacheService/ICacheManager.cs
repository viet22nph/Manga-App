

namespace MangaApp.Application.Abstraction.Services.CacheService;

public interface ICacheManager : IDisposable
{

    Task<string> GetAsync(string cacheKey);
    Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null);
    Task<T> GetAsync<T>(string key);
    T Get<T>(string key, Func<T> acquire, int? cacheTime = null);
    /// <summary>
    /// Increment a field value in a hash
    /// </summary>
    /// <param name="key">Redis key</param>
    /// <param name="data">Data save redis</param>
    /// <param name="cacheTime">FromMinutes cache expired</param>
    Task SetAsync(string key, object data, int cacheTime);
    Task SetAsync<T>(string key, T data, int cacheTime);
    void Set(string key, object data, int cacheTime);
    bool IsSet(string key);
    void Remove(string key);
    void RemoveByPrefix(string prefix);
    void Clear();
}
