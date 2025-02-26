using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Extensions;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangaApp.Domain.Entities;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Chapter.Command;
namespace MangaApp.Application.UserCases.V1.Commands.Chapter;

public class UpdateChapterCommandHandler : ICommandHandler<UpdateChapterCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service _awsS3Service;
    public UpdateChapterCommandHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service)
    {
        _unitOfWork = unitOfWork;
        _awsS3Service = awsS3Service;
    }

    public async  Task<Result<Success>> Handle(UpdateChapterCommand request, CancellationToken cancellationToken)
    {
        var chapter = await _unitOfWork.ChapterRepository
            .FindAll(x=> x.Id== request.Id)
            .AsTracking()
            .Include(x=> x.Images)
            .Include(x=> x.Manga)
            .FirstOrDefaultAsync();
        if (chapter == null)
        {
            return Error.NotFound(nameof(Domain.Entities.Chapter), "Chapter not found.");
        }
        chapter.Update(request.Number, request.Title);
        var updatedImages = new List<ChapterImage>();
        foreach (var image in request.Images)
        {
            if (image.File is not null)
            {
                var imageUrl = await _awsS3Service.UploadFileImageAsync($"{chapter.Manga.Slug}/{request.Title.ToSlug()}/{image.File.FileName}", image.File);
                updatedImages.Add(new ChapterImage { Path = imageUrl, OrderIndex = image.Position, Created = DateTimeOffset.UtcNow });
            }
            else if (!string.IsNullOrEmpty(image.Url))
            {
                var chapterImage = chapter.Images
                    .FirstOrDefault(x => _awsS3Service.ConvertBucketS3ToCloudFront(x.Path) == image.Url);
                if (chapterImage is null) {
                    updatedImages
                      .Add(new ChapterImage
                      {
                          Path = _awsS3Service.ConvertCloudFrontToBucketS3(image.Url),
                          OrderIndex = image.Position,
                          Created = DateTimeOffset.UtcNow
                      });
                }
                else
                {
                    chapterImage!.OrderIndex = image.Position;
                    updatedImages.Add(chapterImage);
                }
            }
        }
        chapter.SetChapterImages(updatedImages);
        _unitOfWork.ChapterRepository.Update(chapter);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ResultType.Success;

    }
}
