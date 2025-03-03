
using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Constants;
using MangApp.Application.Abstraction;
using static MangaApp.Contract.Services.V1.MangaView.Command;

namespace MangaApp.Application.UserCases.V1.Commands.View;

public class IncreaseMangaViewCommandHandler : ICommandHandler<IncreaseMangaViewCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheManager _cacheManager;
    public IncreaseMangaViewCommandHandler(IUnitOfWork unitOfWork, ICacheManager cacheManager)
    {
        _unitOfWork = unitOfWork;
        _cacheManager = cacheManager;
    }

    public async Task<Result<Success>> Handle(IncreaseMangaViewCommand request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var cacheKey = $"{RedisKey.VIEW_MANGA}{request.MangaId}:{today}";
        await _cacheManager.IncrementViewAsync(cacheKey);
        return ResultType.Success;
    }
}
