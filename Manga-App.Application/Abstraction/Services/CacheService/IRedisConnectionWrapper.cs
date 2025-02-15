
using MangaApp.Contract.Shares.Enums;
using StackExchange.Redis;

namespace MangaApp.Application.Abstraction.Services.CacheService;

public interface IRedisConnectionWrapper : IDisposable
{
    IDatabase GetDatabase(int db);

    IServer GetServer(System.Net.EndPoint endPoint);

    System.Net.EndPoint[] GetEndPoints();

    void FlushDatabase(RedisDatabaseNumber db);
}
