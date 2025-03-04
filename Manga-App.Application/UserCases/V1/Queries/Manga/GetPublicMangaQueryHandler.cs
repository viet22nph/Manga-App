
using MangaApp.Application.Abstraction.Services;
using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Dtos.Chapter;
using MangaApp.Contract.Dtos.Country;
using MangaApp.Contract.Dtos.Genre;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Constants;
using MangaApp.Contract.Shares.Enums;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Manga.Query;
using static MangaApp.Contract.Services.V1.Manga.Response;

namespace MangaApp.Application.UserCases.V1.Queries.Manga;

public class GetPublicMangaQueryHandler : IQueryHandler<GetPublicMangaQuery, Pagination<MangaResponse>>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _awsS3Service;
    public readonly ICacheManager _cacheManager;

    public GetPublicMangaQueryHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service, ICacheManager cacheManager)
    {
        _awsS3Service = awsS3Service;
        _unitOfWork = unitOfWork;
        _cacheManager = cacheManager;
    }
    public async Task<Result<Pagination<MangaResponse>>> Handle(GetPublicMangaQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = $"{RedisKey.MANGA_PUBLIC}{request.SearchTerm ?? ""}-{request.GenreSlug ?? ""}-{request.Status.ToString() ?? ""}-{request.PageIndex}-{request.PageSize}-{request.SortColumn ?? ""}-{request.SortOrder ?? ""}";
        var mangaCache =await _cacheManager.GetAsync<Pagination<MangaResponse>>(cacheKey);
        if(mangaCache != null)
        {
            return mangaCache;
        }    
        string sortKeyExpression = request?.SortColumn?.ToLower() switch
        {
            "id" => nameof(Domain.Entities.Manga.Id),
            "title" => nameof(Domain.Entities.Manga.Title),
            _ => nameof(Domain.Entities.Manga.ModifiedDate)
        };
        // sql search term
        string sortOrder = request!.SortOrder == SortOrder.Ascending ? "ASC" : "DESC";

        // Apply filtering based on SearchTerm
        var mangaQueryable = string.IsNullOrWhiteSpace(request.SearchTerm)
          ? _unitOfWork.MangaRepository.FindAll(x=> x.ApprovalStatus == ApprovalStatus.Approved && x.IsPublished == true)
          : _unitOfWork.MangaRepository
          .FindAll( x => x.ApprovalStatus == ApprovalStatus.Approved && x.IsPublished == true && EF.Functions.Collate(x.Title, "Vietnamese_CI_AI").ToLower().Contains(request.SearchTerm.ToLower()) );

        // Apply filtering based on Status and GenreSlug
        if (request.Status != null)
        {
            mangaQueryable = mangaQueryable.Where(x => x.Status == request.Status);
        }

        if (!string.IsNullOrWhiteSpace(request.GenreSlug))
        {
            mangaQueryable = mangaQueryable.Where(x => x.MangaGenres.Any(mg => mg.Genre.Slug == request.GenreSlug));
        }
        // Apply sorting and pagination after filtering
        var sortedMangaQueryable = request.SortOrder == SortOrder.Ascending
           ? mangaQueryable.OrderBy(x => EF.Property<object>(x, sortKeyExpression))
           : mangaQueryable.OrderByDescending(x => EF.Property<object>(x, sortKeyExpression));

        // select data pagiation
        var listMangaResponse = await sortedMangaQueryable
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new MangaResponse
            {
                Id = x.Id,
                Title = x.Title,
                AnotherTitle = x.AnotherTitle,
                Description = x.Description,
                Author = x.Author,
                Thumbnail = _awsS3Service.ConvertBucketS3ToCloudFront(x.Thumbnail),
                Slug = x.Slug,
                Country = new CountryDto { Id = x.Country.Id, Name = x.Country.Name, Code = x.Country.Code },
                Status = x.Status.ToString(),
                ContentRating = x.ContentRating.ToString(),
                Year = x.Year,
                ApprovalStatus = x.ApprovalStatus.ToString(),
                State = x.IsPublished == true ? "published" : "draft",
                Genres = x.MangaGenres.Select(mg => new GenreDto
                {
                    Id = mg.GenreId,
                    Name = mg.Genre.Name,
                    Slug = mg.Genre.Slug,
                    Description = mg.Genre.Description
                }).ToList(),
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                LatestUploadedChapter = x.Chapters.Select(c => new ChapterDto
                {
                    Id = c.Id,
                    Title = $"{c.Number}. {c.Title}",
                    ReleaseDate = c.CreatedDate
                }).OrderByDescending(c => c.Id).FirstOrDefault(),
            })
            .ToListAsync(cancellationToken);
        var totalCountManga = await sortedMangaQueryable.CountAsync(cancellationToken);

        var result = Pagination<MangaResponse>.Create(listMangaResponse, request.PageIndex, request.PageSize, totalCountManga);
        // save cache 
        await _cacheManager.SetAsync<Pagination<MangaResponse>>(cacheKey, result, 5);

        return result;
    }
}
