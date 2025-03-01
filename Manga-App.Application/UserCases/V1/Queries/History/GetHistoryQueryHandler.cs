
using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Dtos.Chapter;
using MangaApp.Contract.Dtos.Country;
using MangaApp.Contract.Dtos.Genre;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Enums;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.History.Query;
using static MangaApp.Contract.Services.V1.Manga.Response;

namespace MangaApp.Application.UserCases.V1.Queries.History;

public sealed class GetHistoryQueryHandler : IQueryHandler<GetHistoryQuery, Pagination<MangaResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _awsS3Service;
    public GetHistoryQueryHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service )
    {
        _unitOfWork = unitOfWork;
        _awsS3Service = awsS3Service;
    }

    public async Task<Result<Pagination<MangaResponse>>> Handle(GetHistoryQuery request, CancellationToken cancellationToken)
    {

        var mangaListQueryable = _unitOfWork
        .HistoryRepository
        .FindAll(x => x.UserId == request.UserId)
        .OrderByDescending(x => x.LastReadAt)
        .Select(x => new MangaResponse
        {
            Id = x.Manga.Id,
            Title = x.Manga.Title,
            AnotherTitle = x.Manga.AnotherTitle,
            Description = x.Manga.Description,
            Author = x.Manga.Author,
            Thumbnail = _awsS3Service.ConvertBucketS3ToCloudFront(x.Manga.Thumbnail),
            Slug = x.Manga.Slug,
            Country = new CountryDto { Id = x.Manga.Country.Id, Name = x.Manga.Country.Name, Code = x.Manga.Country.Code },
            Status = x.Manga.Status.ToString(),
            ContentRating = x.Manga.ContentRating.ToString(),
            Year = x.Manga.Year,
            ApprovalStatus = x.Manga.ApprovalStatus.ToString(),
            State = x.Manga.IsPublished == true ? "published" : "draft",
            Genres = x.Manga.MangaGenres.Select(mg => new GenreDto
            {
                Id = mg.GenreId,
                Name = mg.Genre.Name,
                Slug = mg.Genre.Slug,
                Description = mg.Genre.Description
            }).ToList(),
            CreatedDate = x.Manga.CreatedDate,
            ModifiedDate = x.Manga.ModifiedDate,
            LatestUploadedChapter = x.Manga.Chapters.Select(c => new ChapterDto
            {
                Id = c.Id,
                Title = $"{c.Number}. {c.Title}",
                ReleaseDate = c.CreatedDate
            }).OrderByDescending(c => c.Id).FirstOrDefault(),
        })
        .GroupBy(x => x.Id)
        .Select(x => x.First());
        var mangaList = await mangaListQueryable
            .AsSingleQuery()
            .Skip((request.PageIndex-1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
            var count = await mangaListQueryable.CountAsync(cancellationToken);
        return Pagination<MangaResponse>.Create(mangaList, request.PageIndex, request.PageSize, count);
    }
}
