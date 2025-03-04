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

namespace MangaApp.Application.UserCases.V1.Queries.Manga;

public class GetMangaByIdQueryHandler : IQueryHandler<GetMangaByIdQuery, MangaDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _awsS3Service;
    private readonly ICacheManager _cacheManager;
    public GetMangaByIdQueryHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service, ICacheManager cacheManager)
    {
        _unitOfWork = unitOfWork;
        _awsS3Service = awsS3Service;
        _cacheManager = cacheManager;
    }


    public async Task<Result<MangaDetailResponse>> Handle(GetMangaByIdQuery request, CancellationToken cancellationToken)
    {
        var mangaQueryable =  _unitOfWork.MangaRepository
            .FindAll(x => x.Id == request.Id);

        var data = await mangaQueryable.Select(manga=>  new MangaResponse
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
        }).FirstOrDefaultAsync();
        if (data is null)
        {
            return Error.Failure(code: nameof(Domain.Entities.Manga), description: "Manga not found");
        }
         var viewCount = await _unitOfWork.ViewRepository.FindAll(x => x.MangaId == request.Id).SumAsync(x => x.ViewCount);

        var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var viewCountRedis = await _cacheManager.GetAsync($"{RedisKey.VIEW_MANGA}{data.Id}:{today}");// view chưa cập nhập vào db
        viewCount = viewCountRedis is null ? viewCount : viewCount + int.Parse(viewCountRedis);

        var follow = await _unitOfWork.FollowRepository
            .FindAll(x=> x.MangaId == request.Id)
            .CountAsync();
        var rating = await _unitOfWork.RatingRepository
            .FindAll(x => x.MangaId == request.Id).Select(x=> x.Score).ToListAsync();
        var ratingAvg = rating.Count == 0 ? 0 : rating.Average(x=> x);
        var response = new MangaDetailResponse
        {
            Manga = data,
            Follow = follow,
            Rating = (float)ratingAvg,
            View = viewCount
        };
        return response;
    }
}
