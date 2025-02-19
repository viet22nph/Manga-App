using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
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
    public GetMangaByIdQueryHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service)
    {
        _unitOfWork = unitOfWork;
        _awsS3Service = awsS3Service;
    }


    public async Task<Result<MangaDetailResponse>> Handle(GetMangaByIdQuery request, CancellationToken cancellationToken)
    {
        var manga = await _unitOfWork.MangaRepository
            .FindAll(x => x.Id == request.Id)
            .Include(x=> x.MangaGenres)
                .ThenInclude(y=> y.Genre)
            .Include(x=> x.Country)
            .FirstOrDefaultAsync(cancellationToken);

        if(manga is null)
        {
            return Error.Failure(code: nameof(Domain.Entities.Manga), description: "Manga not found");
        }

        var response = new MangaDetailResponse(
            Id: manga.Id,
            Title: manga.Title,
            AnotherTitle: manga.AnotherTitle,
            Description: manga.Description,
            Author: manga.Author,
            Thumbnail: _awsS3Service.ConvertBucketS3ToCloudFront(manga.Thumbnail),
            Slug: manga.Slug,
            Status: manga.Status.ToString(),
            Year: manga.Year,
            ContentRating: manga.ContentRating.ToString(),
            Genres: manga.MangaGenres.Select(x=> new Contract.Dtos.Genre.Genre
            {
                Id= x.Genre.Id,
                Name= x.Genre.Name,
                Slug = x.Genre.Slug,
            }).ToList(),
            Country: new Contract.Dtos.Country.Country
            {
                Id = manga.Country.Id,
                Name = manga.Country.Name,
                Code = manga.Country.Code,
            },
            CreateDate: manga.CreatedDate,
            UpdateDate: manga.ModifiedDate
        );
        return response;
    }
}
