﻿using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Infrastructure.DependencyInjection.Options;
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

namespace MangaApp.Infrastructure.RedisCache;

public class RedisConnectionWrapper : IRedisConnectionWrapper, ILocker
{
    #region Fields

    private readonly RedisOptions _config;

    private readonly object _lock = new object();
    private volatile ConnectionMultiplexer _connection;
    private readonly Lazy<string> _connectionString;
    private volatile RedLockFactory _redisLockFactory;

    #endregion

    #region Ctor

    public RedisConnectionWrapper(IOptions<RedisOptions> config)
    {
        _config = config.Value;
        _connectionString = new Lazy<string>(GetConnectionString);
        _redisLockFactory = CreateRedisLockFactory();
    }

    #endregion

    #region Utilities


    protected string GetConnectionString()
    {
        return _config.RedisConnectionString;
    }


    protected ConnectionMultiplexer GetConnection()
    {
        if (_connection != null && _connection.IsConnected) return _connection;

        lock (_lock)
        {
            if (_connection != null && _connection.IsConnected) return _connection;

            //Connection disconnected. Disposing connection...
            _connection?.Dispose();

            //Creating new instance of Redis Connection
            _connection = ConnectionMultiplexer.Connect(_connectionString.Value);
        }

        return _connection;
    }


    protected RedLockFactory CreateRedisLockFactory()
    {
        //get RedLock endpoints
        var configurationOptions = ConfigurationOptions.Parse(_connectionString.Value);
        var redLockEndPoints = GetEndPoints().Select(endPoint => new RedLockEndPoint
        {
            EndPoint = endPoint,
            Password = configurationOptions.Password,
            Ssl = configurationOptions.Ssl,
            RedisDatabase = configurationOptions.DefaultDatabase,
            ConfigCheckSeconds = configurationOptions.ConfigCheckSeconds,
            ConnectionTimeout = configurationOptions.ConnectTimeout,
            SyncTimeout = configurationOptions.SyncTimeout
        }).ToList();

        //create RedLock factory to use RedLock distributed lock algorithm
        return RedLockFactory.Create(redLockEndPoints);
    }

    #endregion

    #region Methods

    public IDatabase GetDatabase(int db)
    {
        return GetConnection().GetDatabase(db);
    }

    public IServer GetServer(System.Net.EndPoint endPoint)
    {
        return GetConnection().GetServer(endPoint);
    }
    public System.Net.EndPoint[] GetEndPoints()
    {
        return GetConnection().GetEndPoints();
    }

    public void FlushDatabase(RedisDatabaseNumber db)
    {
        var endPoints = GetEndPoints();

        foreach (var endPoint in endPoints)
        {
            GetServer(endPoint).FlushDatabase((int)db);
        }
    }

    public bool PerformActionWithLock(string resource, TimeSpan expirationTime, Action action)
    {
        //use RedLock library
        using (var redisLock = _redisLockFactory.CreateLock(resource, expirationTime))
        {
            //ensure that lock is acquired
            if (!redisLock.IsAcquired)
                return false;

            //perform action
            action();

            return true;
        }
    }

    public void Dispose()
    {
        //dispose ConnectionMultiplexer
        _connection?.Dispose();

        //dispose RedLock factory
        _redisLockFactory?.Dispose();
    }
    #endregion
}
