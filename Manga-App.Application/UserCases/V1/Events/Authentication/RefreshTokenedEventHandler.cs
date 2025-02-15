
using Manga_App.Contract.Abstractions.Messages;
using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Contract.Shares.Constants;
using static MangaApp.Contract.Services.V1.Authentication.Event;

namespace MangaApp.Application.UserCases.V1.Events.Authentication;

public class RefreshTokenedEventHandler : IEventHandler<RefreshTokenedEvent>
{
    private readonly ICacheManager _cacheManager;
    public RefreshTokenedEventHandler(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }
    public async Task Handle(RefreshTokenedEvent notification, CancellationToken cancellationToken)
    {
        string key = $"{RedisKey.LIST_REFRESH_KEY}{notification.UserId}-{notification.Device}";
        int cacheTime = (int)TimeSpan.FromDays(7).TotalMinutes;
        await _cacheManager.SetAsync(key, notification.RefreshToken, cacheTime);
    }
}
