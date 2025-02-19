
using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Enums;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Manga.Query;
using static MangaApp.Contract.Services.V1.Manga.Response;

namespace MangaApp.Application.UserCases.V1.Queries.Manga;

public class GetPublicMangaQueryHandler : IQueryHandler<GetPublicMangaQuery, Pagination<MangaPublicResponse>>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _awsS3Service;
    public GetPublicMangaQueryHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service)
    {
        _awsS3Service = awsS3Service;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Pagination<MangaPublicResponse>>> Handle(GetPublicMangaQuery request, CancellationToken cancellationToken)
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
            .Select(x => new MangaPublicResponse(
                x.Id,
                x.Title,
                x.AnotherTitle,
                x.Description,
                x.Author,
                _awsS3Service.ConvertBucketS3ToCloudFront(x.Thumbnail),
                x.Slug,
                x.Status.ToString(),
                x.Year,
                x.MangaGenres.Select(y => y.Genre.Name).ToList(),
                x.Country!.Name,
                x.ContentRating == ContentRating.Safe || x.ContentRating == ContentRating.Suggestive ? false : true,
                x.ContentRating.ToString(),
                x.CreatedDate,
                x.ModifiedDate))
            .ToListAsync(cancellationToken);
        var totalCountManga = await sortedMangaQueryable.CountAsync(cancellationToken);

        return Pagination<MangaPublicResponse>.Create(listMangaResponse, request.PageIndex, request.PageSize, totalCountManga);
    }
}
