
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Services.V1.Chapter;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangApp.Application.Abstraction;
using Microsoft.EntityFrameworkCore;
using static MangaApp.Contract.Services.V1.Chapter.Query;
using static MangaApp.Contract.Services.V1.Chapter.Response;

namespace MangaApp.Application.UserCases.V1.Queries.Chapter;

public class GetListChapterQueryHanlder : IQueryHandler<GetListChapterQuery, Pagination<ChapterResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetListChapterQueryHanlder(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Pagination<ChapterResponse>>> Handle(GetListChapterQuery request, CancellationToken cancellationToken)
    {
        string sortKeyExpression = request?.SortColumn?.ToLower() switch
        {
            nameof(Domain.Entities.Chapter.Id) => nameof(Domain.Entities.Chapter.Id),
            nameof(Domain.Entities.Chapter.Title) => nameof(Domain.Entities.Chapter.Title),
            _ => nameof(Domain.Entities.Chapter.Number)
        };

        string sortOrder = request!.SortOrder == SortOrder.Ascending ? "ASC" : "DESC";
        var chapterQueryable = request.MangaId is null
          ? _unitOfWork.ChapterRepository.FindAll()
          : _unitOfWork.ChapterRepository
          .FindAll(x=> x.MangaId == request.MangaId);


        // Apply sorting and pagination after filtering
        var sortedchapterQueryable = request.SortOrder == SortOrder.Ascending
            ? chapterQueryable.OrderBy(x => EF.Property<object>(x, sortKeyExpression))
            : chapterQueryable.OrderByDescending(x => EF.Property<object>(x, sortKeyExpression));
        var listChapterResponse = await sortedchapterQueryable
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(chapter=>
                new ChapterResponse(
                     chapter.Id,
                     nameof(Domain.Entities.Chapter),
                     new ChapterAttributes(
                          chapter.Number,
                          chapter.MangaId,
                          chapter.Title,
                          chapter.CreatedDate,
                          chapter.ModifiedDate,
                          chapter.Images.Count()
                     )
                )
            )
            .ToListAsync(cancellationToken);
        var count = await sortedchapterQueryable.CountAsync(cancellationToken);
        return  Pagination<ChapterResponse>.Create(listChapterResponse, request.PageIndex, request.PageSize, count);
    }
}
