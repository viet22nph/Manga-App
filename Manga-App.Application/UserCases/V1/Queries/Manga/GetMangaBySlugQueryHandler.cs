
using MangaApp.Application.Abstraction.Services;
using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Dtos.Chapter;
using MangaApp.Contract.Dtos.Country;
using MangaApp.Contract.Dtos.Genre;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Constants;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Manga.Query;
using static MangaApp.Contract.Services.V1.Manga.Response;
using Error = MangaApp.Contract.Shares.Errors.Error;

namespace MangaApp.Application.UserCases.V1.Queries.Manga;

public class GetMangaBySlugQueryHandler : IQueryHandler<GetMangaBySlugQuery, MangaDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _awsS3Service;
    private readonly ICacheManager _cacheManager;
    public GetMangaBySlugQueryHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service, ICacheManager cacheManager)
    {
        _awsS3Service = awsS3Service;
        _unitOfWork = unitOfWork;
        _cacheManager = cacheManager;
    }
    public async Task<Result<MangaDetailResponse>> Handle(GetMangaBySlugQuery request, CancellationToken cancellationToken)
    {
        string redisKey = $"{RedisKey.MANGA}{request.Slug}";
        var mangaCache = await _cacheManager.GetAsync<MangaDetailResponse>(redisKey);
        if(mangaCache != null)
        {
            mangaCache.View = await GetUpdatedViewCountAsync(mangaCache.Manga.Id);
            mangaCache.Follow = await GetNumberFollowMangaAsync(mangaCache.Manga.Id);
            mangaCache.Rating = await GetAverageRatingAsync(mangaCache.Manga.Id);
            return mangaCache;
        }    
        

        var manga = await _unitOfWork.MangaRepository
            .FindAll(x=> x.Slug.Equals(request.Slug))
            .Select(manga => new MangaResponse
            {
                Id = manga.Id,
                Title = manga.Title,
                AnotherTitle = manga.AnotherTitle,
                Description = manga.Description,
                Author = manga.Author,
                Thumbnail = _awsS3Service.ConvertBucketS3ToCloudFront(manga.Thumbnail),
                Slug = manga.Slug,
                Country = new CountryDto { Id = manga.Country.Id, Name = manga.Country.Name, Code = manga.Country.Code },
                Status = manga.Status.ToString(),
                ContentRating = manga.ContentRating.ToString(),
                Year = manga.Year,
                ApprovalStatus = manga.ApprovalStatus.ToString(),
                State = manga.IsPublished == true ? "published" : "draft",
                Genres = manga.MangaGenres.Select(mg => new GenreDto
                {
                   Id = mg.GenreId,
                   Name = mg.Genre.Name,
                   Slug = mg.Genre.Slug,
                   Description = mg.Genre.Description
                }).ToList(),
                CreatedDate = manga.CreatedDate,
                ModifiedDate = manga.ModifiedDate,
                LatestUploadedChapter = manga.Chapters.Select(c => new ChapterDto
                {
                   Id = c.Id,
                   Title = $"{c.Number}. {c.Title}",
                   ReleaseDate = c.CreatedDate
                }).OrderByDescending(c => c.Id).FirstOrDefault(),
            })
            .FirstOrDefaultAsync();

        if (manga is null)
        {
            return Error.Failure(code: nameof(Domain.Entities.Manga), description: "Manga not found");
        }
        var viewCount =  await GetUpdatedViewCountAsync(manga.Id);
        var follow = await GetNumberFollowMangaAsync(manga.Id);
        var rating = await GetAverageRatingAsync(manga.Id);



        var response = new MangaDetailResponse
        {
            Manga = manga,
            Follow = follow,
            Rating = (float)rating,
            View = (int)viewCount
        };
        await _cacheManager.SetAsync<MangaDetailResponse>(redisKey, response, 5); // cache 5 p.
        return response;
    }

    /// <summary>
    /// Lấy tổng lượt view từ DB + Redis (view của ngày hiện tại chưa cập nhật vào DB)
    /// </summary>
    private async Task<long> GetUpdatedViewCountAsync(Guid mangaId)
    {
        string today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        string viewRedisKey = $"{RedisKey.VIEW_MANGA}{mangaId}:{today}";

        // Lấy view từ DB
        var viewCount = await _unitOfWork.ViewRepository
            .FindAll(x => x.MangaId == mangaId)
            .SumAsync(x => x.ViewCount);

        // Lấy view từ Redis (view chưa cập nhật vào DB)
        var viewCountFromRedis = await _cacheManager.GetAsync<string>(viewRedisKey);
        int additionalViews = string.IsNullOrEmpty(viewCountFromRedis) ? 0 : int.Parse(viewCountFromRedis);

        return viewCount + additionalViews;
    }
    private async Task<int> GetNumberFollowMangaAsync(Guid mangaId)
    {
       return await _unitOfWork.FollowRepository
           .FindAll(x => x.MangaId == mangaId)
           .CountAsync();
    }
    private async Task<float> GetAverageRatingAsync(Guid mangaId)
    {
        var rating = await _unitOfWork.RatingRepository
            .FindAll(x => x.MangaId == mangaId).Select(x => x.Score).ToListAsync();
        return rating.Count == 0 ? 0 : (float)rating.Average(x => x);
    }

}
