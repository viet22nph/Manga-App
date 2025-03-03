

using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Constants;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.MangaView.Query;
using static MangaApp.Contract.Services.V1.MangaView.Response;

namespace MangaApp.Application.UserCases.V1.Queries.View;

public class GetViewMangaQueryHandler : IQueryHandler<GetViewMangaQuery, ViewMangaResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheManager _cacheManager;
    public GetViewMangaQueryHandler(IUnitOfWork unitOfWork, ICacheManager cacheManager)
    {
        _unitOfWork = unitOfWork;
        _cacheManager = cacheManager;
    }


    public async Task<Result<ViewMangaResponse>> Handle(GetViewMangaQuery request, CancellationToken cancellationToken)
    {
        // Get View by database
        var viewCount = await _unitOfWork.ViewRepository.FindAll(x=> x.MangaId == request.MangaId).SumAsync(x=> x.ViewCount);

        var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var viewRedis = await _cacheManager.GetAsync($"{RedisKey.VIEW_MANGA}{request.MangaId}:{today}");

        return new ViewMangaResponse(viewCount);
    }
}
