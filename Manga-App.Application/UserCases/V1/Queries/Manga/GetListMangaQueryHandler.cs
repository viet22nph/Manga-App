
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Contract.Shares;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Manga.Query;
using static MangaApp.Contract.Services.V1.Manga.Response;
using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Dtos.Country;
using MangaApp.Contract.Dtos.Genre;
using MangaApp.Contract.Dtos.Chapter;

namespace MangaApp.Application.UserCases.V1.Queries.Manga;

public class GetListMangaQueryHandler : IQueryHandler<GetListMangaQuery, Pagination<MangaResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _awsS3Service;
    public GetListMangaQueryHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service)
    {
        _awsS3Service = awsS3Service;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Pagination<MangaResponse>>> Handle(GetListMangaQuery request, CancellationToken cancellationToken)
    {
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
          ? _unitOfWork.MangaRepository.FindAll()
          : _unitOfWork.MangaRepository
          .FindAll(x=> EF.Functions.Collate(x.Title, "Vietnamese_CI_AI").ToLower().Contains(request.SearchTerm.ToLower()));

        // Apply filtering based on Status and GenreSlug
        if (request.Status != null)
        {
            mangaQueryable = mangaQueryable.Where(x => x.Status == request.Status);
        }

        if (!string.IsNullOrWhiteSpace(request.GenreSlug))
        {
            mangaQueryable = mangaQueryable.Where(x => x.MangaGenres.Any(mg => mg.Genre.Slug == request.GenreSlug));
        }

        if(request.Approval != null)
        {
            mangaQueryable = mangaQueryable.Where(x => x.ApprovalStatus == request.Approval);
        }
        
        if(request.IsPublished != null)
        {
            mangaQueryable = mangaQueryable.Where(x => x.IsPublished == request.IsPublished);
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
                State = x.IsPublished == true? "published": "draft",
                Genres = x.MangaGenres.Select(mg=> new GenreDto
                {
                    Id = mg.GenreId,
                    Name = mg.Genre.Name,
                    Slug = mg.Genre.Slug,
                    Description = mg.Genre.Description
                }).ToList(),
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                LatestUploadedChapter = x.Chapters.Select(c=> new ChapterDto
                {
                    Id = c.Id,
                    Title = $"{c.Number}. {c.Title}",
                    ReleaseDate = c.CreatedDate
                }).OrderByDescending(c=> c.Id).FirstOrDefault(),
            })
            .ToListAsync(cancellationToken);
        var totalCountManga = await sortedMangaQueryable.CountAsync(cancellationToken);

        return Pagination<MangaResponse>.Create(listMangaResponse, request.PageIndex, request.PageSize, totalCountManga);
    }
}