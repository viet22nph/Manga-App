using MangaApp.Application.Security.Attributes;
using MangaApp.Contract.Extensions;
using MangaApp.Contract.Security.Permissions;
using MangaApp.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MangaApp.Contract.Services.V1.Chapter.Command;
using static MangaApp.Contract.Services.V1.Chapter.Query;
namespace MangaApp.Presentation.Controllers.V1;

[Route("api/v{version:apiVersion}/chapter")] // Version in URL
[ApiVersion("1.0")]
public class ChapterController : ApiController
{
    public ChapterController(ISender sender) : base(sender)
    {
    }
    [CheckAccess(permission: Permission.Chapter.Create)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateManga([FromForm] CreateChapterCommand request)
    {

        var result = await _sender.Send(request);
        return result.Match(_ => Created(), Problem);
    }
    [HttpGet]
    public async Task<IActionResult> GetListChapter(
        [FromQuery] Guid? manga,
        [FromQuery] string? sortColumn,
        [FromQuery] string? orderBy,
        [FromQuery] int? pageIndex,
        [FromQuery] int? pageSize
        )
    {
        if (pageIndex <= 0)
        {
            pageIndex = 1;
        }
        if (pageSize <= 0 || pageSize >= 100)
        {
            pageSize = 10;
        }
        var sort = orderBy!.GetSortOrder();
        var query = new GetListChapterQuery(manga, sortColumn, sort, pageIndex ?? 1, pageSize ?? 10);

        var rs = await _sender.Send(query);
        return rs.Match(data => Ok(new
        {
            result = "ok",
            response ="collection",
            data = data.Data,
            pageIndex = data.PageIndex,
            pageSize = data.PageSize,
            total = data.TotalCount
        }), Problem);
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetChapterById(Guid id)
    {
        var query = new GetChapterByIdQuery(id);    
        var rs = await _sender.Send(query);
        return rs.Match(data => Ok(new
        {
            result = "ok",
            response = "entity",
            data = data,
        }), Problem);
    }
    [HttpPut("{id:guid}")]
    [CheckAccess(permission: Permission.Chapter.Update)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateChapter(Guid id, [FromForm] UpdateChapterCommand request)
    {
        if (id != request.Id)
        {
            return BadRequest("Manga ID does not match.");
        }

        var result = await _sender.Send(request);

        return result.Match(
            _ => NoContent(),
           Problem
        );
    }
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetChapterBySlug(string slug)
    {
        var query = new GetChapterBySlugQuery(slug);
        var rs = await _sender.Send(query);
        return rs.Match(data => Ok(new
        {
            result = "ok",
            response = "entity",
            data = data,
        }), Problem);
    }
}
