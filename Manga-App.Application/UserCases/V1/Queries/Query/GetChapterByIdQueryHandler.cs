
using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Services.V1.Chapter;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using System.Linq;
using static MangaApp.Contract.Services.V1.Chapter.Query;
using static MangaApp.Contract.Services.V1.Chapter.Response;

namespace MangaApp.Application.UserCases.V1.Queries.Query;

public class GetChapterByIdQueryHandler : IQueryHandler<GetChapterByIdQuery, ChapterDetailReponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAwsS3Service  _awsS3Service;
    public GetChapterByIdQueryHandler(IUnitOfWork unitOfWork, IAwsS3Service awsS3Service)
    {
        _awsS3Service = awsS3Service;
        _unitOfWork = unitOfWork;
    }


    public async Task<Result<ChapterDetailReponse>> Handle(GetChapterByIdQuery request, CancellationToken cancellationToken)
    {
        var chapter = await _unitOfWork.ChapterRepository.FindByIdAsync(request.ChapterId, cancellationToken, x=> x.Images);
        if(chapter is null)
        {
            return Error.Failure(code: nameof(Domain.Entities.Chapter), description:"Chapter not found");
        }

        var chapterResponse = new ChapterDetailReponse(
            Id: chapter.Id,
            Type: nameof(Domain.Entities.Chapter),
            new ChapterAttributesDetail(
                chapter.Number,
                chapter.MangaId,
                chapter.Title,
                chapter.Images.OrderBy(x=> x.OrderIndex).Select(x=>_awsS3Service.ConvertBucketS3ToCloudFront(x.Path)).ToList(),
                chapter.CreatedDate,
                chapter.ModifiedDate,
                chapter.Images.Count()
                )
            );
        return chapterResponse;
    }
}
