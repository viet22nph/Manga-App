
using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Contract.Shares.Constants;
using MangApp.Application.Abstraction;
using MassTransit.Mediator;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading;
using static MangaApp.Contract.Services.V1.MangaView.Command;

namespace MangaApp.Application.Background;

public class SyncMangaViewJob : IJob
{
    private readonly ILogger<SyncMangaViewJob> _logger;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheManager _cacheManager;
    public SyncMangaViewJob(ILogger<SyncMangaViewJob> logger, IUnitOfWork unitOfWork, ICacheManager cacheManager)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cacheManager = cacheManager;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting SyncMangaViewJob...");
        var views = await _cacheManager.GetAllViewsAsync();
        foreach (var (mangaId, count) in views)
        {
            var today = DateTime.UtcNow.Date;
            var existingView = await _unitOfWork.ViewRepository.FindSingleAsync(x => x.MangaId == mangaId && x.ViewDate == today);

            if (existingView == null)
            {
                _unitOfWork.ViewRepository.Add(new Domain.Entities.MangaViews
                {
                    MangaId = mangaId,
                    ViewDate = today,
                    ViewCount = (uint)count
                });
            }
            else
            {
                existingView.ViewCount += (uint)count;
                _unitOfWork.ViewRepository.Update(existingView);
            }
        }
        await _unitOfWork.SaveChangesAsync();
        _cacheManager.RemoveByPrefix($"{RedisKey.VIEW_MANGA}*");
        _logger.LogInformation("SyncMangaViewJob completed successfully.");
    }
}
