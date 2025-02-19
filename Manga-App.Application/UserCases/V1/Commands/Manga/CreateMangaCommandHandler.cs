

using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Extensions;
using MangaApp.Contract.Services.V1.Manga;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;

namespace MangaApp.Application.UserCases.V1.Commands.Manga;

public sealed class CreateMangaCommandHandler : ICommandHandler<Command.CreateMangaCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _s3Service;
    public CreateMangaCommandHandler(IUnitOfWork unitOfWork, IAwsS3Service s3Service)
    {
        _unitOfWork = unitOfWork;
        _s3Service = s3Service;
    }


    public async Task<Result<Success>> Handle(Command.CreateMangaCommand request, CancellationToken cancellationToken)
    {
        var checkTitleExists = await _unitOfWork.MangaRepository.FindSingleAsync(x => x.Title == request.Title);
        if (checkTitleExists is not null)
        {
            return Error.Validation(code: nameof(Domain.Entities.Manga.Title), description: "Title manga is exists");
        }
        string thumbnailPath = string.Empty;
        try
        {
            thumbnailPath = await _s3Service.UploadFileImageAsync(request.Slug ?? request.Title.ToSlug(), request.Thumbnail);
        }
        catch (Exception ex)
        {
            return Error.Failure(code: nameof(Domain.Entities.Manga.Thumbnail), description: ex.Message);
        }

        var manga = Domain.Entities.Manga.Create(
            title: request.Title,
            anotherTitle: request.AnotherTitle,
            thumbnail: thumbnailPath,
            description: request.Description,
            author: request.Author,
            countryId: request.CountryId,
            status: request.Status,
            contentRating: request.ContentRating,
            slug: request.Slug ?? request.Title.ToSlug(),
            year: request.Year
            );
        // set list genre in manga
        manga.SetStoryGenresByGenreIds(request.GenresId);
        _unitOfWork.MangaRepository.Add(manga);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultType.Success;
    }

}
