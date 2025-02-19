

using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Extensions;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Manga.Command;

namespace MangaApp.Application.UserCases.V1.Commands.Manga;

public class UpdateMangaCommandHandler : ICommandHandler<UpdateMangaCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _awsS3Service;
    public UpdateMangaCommandHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service)
    {
        _unitOfWork = unitOfWork;
        _awsS3Service = awsS3Service;
    }
    public async Task<Result<Success>> Handle(UpdateMangaCommand request, CancellationToken cancellationToken)
    {
        var manga = await _unitOfWork.MangaRepository
            .FindAll(x => x.Id == request.Id)
            .Include(x => x.MangaGenres)
            .AsTracking()
            .FirstOrDefaultAsync();
        if(manga == null)
        {
            return Error.Failure(code: nameof(Domain.Entities.Manga), description: "Manga not found");
        }
        if (!string.IsNullOrEmpty(request.Title) && request.Title != manga.Title)
        {
            var checkTitleExists = await _unitOfWork.MangaRepository.FindSingleAsync(x => x.Title == request.Title && x.Id != request.Id);
            if (checkTitleExists is not null)
            {
                return Error.Validation(nameof(Domain.Entities.Manga.Title), "Title manga already exists");
            }
        }
        string thumbnailPath = manga.Thumbnail;
        try
        {
            if (request.Thumbnail != null)
            {

                thumbnailPath = await _awsS3Service.UploadFileImageAsync(request.Slug ?? request!.Title.ToSlug(), request.Thumbnail);
            }
        }
        catch (Exception ex)
        {
            return Error.Failure(code: nameof(Domain.Entities.Manga.Thumbnail), description: ex.Message);
        }

      
        manga.Update(
            title: request.Title,
            anotherTitle: request.AnotherTitle,
            description: request.Description,
            author: request.Author,
            thumbnail: thumbnailPath,
            countryId: request.CountryId,
            status: request.Status,
            contentRating: request.ContentRating,
            slug: request.Slug ?? request.Title.ToSlug(),
            year: request.Year
        );
        manga.SetStoryGenresByGenreIds(request.GenresId);

        _unitOfWork.MangaRepository.Update(manga);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultType.Success;
    }
}
