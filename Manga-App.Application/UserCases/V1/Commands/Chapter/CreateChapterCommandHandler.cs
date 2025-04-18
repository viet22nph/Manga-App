﻿

using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Extensions;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangaApp.Domain.Entities;
using MangApp.Application.Abstraction;
using MassTransit;
using static MangaApp.Contract.Services.V1.Chapter.Command;
using static MangaApp.Contract.Services.V1.Chapter.Event;

namespace MangaApp.Application.UserCases.V1.Commands.Chapter;

public class CreateChapterCommandHandler : ICommandHandler<CreateChapterCommand, Success>
{
    private readonly IUnitOfWork _unitOfWork;
    private IAwsS3Service _awsS3Service;

    private readonly IPublishEndpoint _publishEndpoint;
    public CreateChapterCommandHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service, IPublishEndpoint publishEndpoint)
    {
        _unitOfWork = unitOfWork;
        _awsS3Service = awsS3Service;
        _publishEndpoint = publishEndpoint;
    }



    public async  Task<Result<Success>> Handle(CreateChapterCommand request, CancellationToken cancellationToken)
    {
        var manga = await _unitOfWork.MangaRepository.FindByIdAsync(request.MangaId);
        if(manga is null)
        {
            return Error.Failure(code: nameof(Domain.Entities.Manga), description: "Manga not found");
        }

        var imagesUrl = new List<ChapterImage>();

        int page = 1;
        try
        {

            foreach (var file in request.PageOrder)
            {

                var imageUrl = await _awsS3Service.UploadFileImageAsync($"{manga.Slug}/{request.Title.ToSlug()}/{file.FileName}", file);
                imagesUrl.Add(new ChapterImage { Path = imageUrl, OrderIndex = page, Created= DateTimeOffset.UtcNow});
                page++;
            }
        }
        catch (Exception ex) { 
            return Error.Failure(code: nameof(request.PageOrder), description: ex.Message);
        }

        var chapter = Domain.Entities.Chapter.Create(request.MangaId, request.Number, request.Title);
        chapter.SetChapterImages(imagesUrl);
        manga.ModifiedDate = DateTimeOffset.UtcNow;
        _unitOfWork.MangaRepository.Update(manga);// cập nhật ngày
            _unitOfWork.ChapterRepository.Add(chapter);
        await _unitOfWork.SaveChangesAsync();

        // using message broker save message
        await _publishEndpoint.Publish(new NewChapterCreatedConsumer
        {
            ChapterId = chapter.Id,
            MangaId = manga.Id,
            MangaName =manga.Title,
            Title = chapter.Title,
            MangaSlug =manga.Slug,
            CreatedAt = DateTime.UtcNow,
        });
        return ResultType.Success;
    }
}
