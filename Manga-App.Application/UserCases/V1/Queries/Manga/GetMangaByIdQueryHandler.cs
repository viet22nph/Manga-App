using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Dtos.Chapter;
using MangaApp.Contract.Dtos.Country;
using MangaApp.Contract.Dtos.Genre;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Manga.Query;
using static MangaApp.Contract.Services.V1.Manga.Response;

namespace MangaApp.Application.UserCases.V1.Queries.Manga;

public class GetMangaByIdQueryHandler : IQueryHandler<GetMangaByIdQuery, MangaResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _awsS3Service;
    public GetMangaByIdQueryHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service)
    {
        _unitOfWork = unitOfWork;
        _awsS3Service = awsS3Service;
    }


    public async Task<Result<MangaResponse>> Handle(GetMangaByIdQuery request, CancellationToken cancellationToken)
    {
        var mangaQueryable =  _unitOfWork.MangaRepository
            .FindAll(x => x.Id == request.Id);

        var response = await mangaQueryable.Select(manga=>  new MangaResponse
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
        if (response is null)
        {
            return Error.Failure(code: nameof(Domain.Entities.Manga), description: "Manga not found");
        }
        return response;
    }
}
