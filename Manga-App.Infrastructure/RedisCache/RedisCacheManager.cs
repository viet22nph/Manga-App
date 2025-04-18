﻿

using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Contract.Shares.Enums;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using StackExchange.Redis;
using MangaApp.Infrastructure.DependencyInjection.Options;
using System.Net;
using Microsoft.Extensions.Options;

namespace MangaApp.Infrastructure.RedisCache;

public class RedisCacheManager : ICacheManager
{
    #region Fields

    private readonly IRedisConnectionWrapper _connectionWrapper;
    private readonly IDatabase _db;
    private readonly RedisOptions _config;

    #endregion

    #region Ctor

    public RedisCacheManager(IRedisConnectionWrapper connectionWrapper, IOptions<RedisOptions> config)
    {

        _config = config.Value;
        if (string.IsNullOrEmpty(_config.RedisConnectionString))
            throw new Exception("Redis connection string is empty");

        _connectionWrapper = connectionWrapper;

        _db = _connectionWrapper.GetDatabase(_config.RedisDatabaseId ?? (int)RedisDatabaseNumber.Cache);

    }

    #endregion
    #region Utilities

    protected virtual IEnumerable<RedisKey> GetKeys(EndPoint endPoint, string prefix = null)
    {
        var server = _connectionWrapper.GetServer(endPoint);

        var keys = server.Keys(_db.Database, string.IsNullOrEmpty(prefix) ? null : $"{prefix}*");

        keys = keys.Where(key => !key.ToString().Equals(_config.RedisDataProtectionKey, StringComparison.OrdinalIgnoreCase));

        return keys;
    }

    public virtual async Task<T> GetAsync<T>(string key)
    {
        //get serialized item from cache
        var serializedItem = await _db.StringGetAsync(key);
        if (!serializedItem.HasValue)
            return default(T);

        //deserialize item
        var item = JsonConvert.DeserializeObject<T>(serializedItem);
        if (item == null)
            return default(T);

        return item;
    }

    protected virtual async Task<bool> IsSetAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }

    #endregion


    #region Methods

    public async Task<string> GetAsync(string cacheKey)
    {
        var cachedResponse = await _db.StringGetAsync(cacheKey);

        if (cachedResponse.IsNullOrEmpty)
        {
            return null;
        }

        return cachedResponse;
    }

    public async Task<T> GetAsync<T>(string key, Func<Task<T>> acquire, int? cacheTime = null)
    {
        //item already is in cache, so return it
        if (await IsSetAsync(key))
            return await GetAsync<T>(key);

        //or create it using passed function
        var result = await acquire();

        //and set in cache (if cache time is defined)
        if ((cacheTime ?? _config.CacheTime) > 0)
            await SetAsync(key, result, cacheTime ?? _config.CacheTime);

        return result;
    }

    public virtual T Get<T>(string key)
    {

        //get serialized item from cache
        var serializedItem = _db.StringGet(key);
        if (!serializedItem.HasValue)
            return default(T);

        //deserialize item
        var item = JsonConvert.DeserializeObject<T>(serializedItem);
        if (item == null)
            return default(T);


        return item;
    }

    public virtual T Get<T>(string key, Func<T> acquire, int? cacheTime = null)
    {
        //item already is in cache, so return it
        if (IsSet(key))
            return Get<T>(key);

        //or create it using passed function
        var result = acquire();

        //and set in cache (if cache time is defined)
        if ((cacheTime ?? _config.CacheTime) > 0)
            Set(key, result, cacheTime ?? _config.CacheTime);

        return result;
    }

    public async Task SetAsync(string key, object data, int cacheTime)
    {
        if (data == null)
            return;

        //set cache time
        var expiresIn = TimeSpan.FromMinutes(cacheTime);

        //serialize item
        var serializedItem = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        });

        //and set it to cache
        await _db.StringSetAsync(key, serializedItem, expiresIn);
    }
    public async Task SetAsync<T>(string key, T data, int cacheTime)
    {
        if (data == null)
            return;

        //set cache time
        var expiresIn = TimeSpan.FromMinutes(cacheTime);

        //serialize item
        var serializedItem = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        });

        //and set it to cache
        await _db.StringSetAsync(key, serializedItem, expiresIn);
    }
    public virtual void Set(string key, object data, int cacheTime)
    {
        if (data == null)
            return;

        //set cache time
        var expiresIn = TimeSpan.FromMinutes(cacheTime);

        //serialize item
        var serializedItem = JsonConvert.SerializeObject(data);

        //and set it to cache
        _db.StringSet(key, serializedItem, expiresIn);
    }

    public virtual bool IsSet(string key)
    {

        return _db.KeyExists(key);
    }

    public virtual void Remove(string key)
    {
        //we should always persist the data protection key list
        if (key.Equals(_config.RedisDataProtectionKey, StringComparison.OrdinalIgnoreCase))
            return;

        //remove item from caches
        _db.KeyDelete(key);
    }

    public virtual void RemoveByPrefix(string prefix)
    {

        foreach (var endPoint in _connectionWrapper.GetEndPoints())
        {
            var keys = GetKeys(endPoint, prefix);

            _db.KeyDelete(keys.ToArray());
        }
    }

    public virtual void Clear()
    {
        foreach (var endPoint in _connectionWrapper.GetEndPoints())
        {
            var keys = GetKeys(endPoint).ToArray();

            _db.KeyDelete(keys);
        }
    }

    public virtual void Dispose()
    {
        if (_connectionWrapper != null)
        {
            _connectionWrapper.Dispose();
        }
        Dispose(true);
    }

    private void Dispose(bool clear)
    {
        GC.SuppressFinalize(this);
    }

    public async Task<long> IncrementViewAsync(string key, long value = 1)
    {
        if (_db == null)
            throw new InvalidOperationException("Redis connection is not initialized.");

        return await _db.StringIncrementAsync(key, value);
    }
    public async Task<Dictionary<Guid, long>> GetAllViewsAsync()
    {
        var views = new Dictionary<Guid, long>();

        foreach (var endPoint in _connectionWrapper.GetEndPoints())
        {
            var keys = GetKeys(endPoint, $"{Contract.Shares.Constants.RedisKey.VIEW_MANGA}*");

            foreach (var key in keys)
            {
                var parts = key.ToString().Split(':');
                if (parts.Length == 3 && Guid.TryParse(parts[1], out Guid mangaId))
                {
                    long count = (long)await _db.StringGetAsync(key);
                    views[mangaId] = count;
                }
            }
        }
        return views;
    }
    #endregion
}
