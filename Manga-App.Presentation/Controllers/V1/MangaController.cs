

using MangaApp.Application.Security.Attributes;
using MangaApp.Contract.Dtos;
using MangaApp.Contract.Extensions;
using MangaApp.Contract.Security.Permissions;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MangaApp.Contract.Services.V1.History.Command;
using static MangaApp.Contract.Services.V1.History.Query;
using static MangaApp.Contract.Services.V1.Manga.Command;
using static MangaApp.Contract.Services.V1.Manga.Query;

namespace MangaApp.Presentation.Controllers.V1;
[Route("api/v{version:apiVersion}/manga")] // Version in URL
[ApiVersion("1.0")]

public class MangaController : ApiController
{
    public MangaController(ISender sender) : base(sender)
    {
    }

    [CheckAccess(permission: Permission.Manga.Create)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateManga([FromForm] CreateMangaCommand request)
    {

        var result = await _sender.Send(request);
        return result.Match(_ => Created(), Problem);
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListManga(
        string? genreSlug,
        MangaStatus? status,
        string? searchTerm = null,
        string? sortColumn = null,
        string? orderBy = null,
        ApprovalStatus? approval = null,
        bool? isPublished = null,
        int? pageIndex = 1,
        int? pageSize = 10)
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
        var query = new GetListMangaQuery(genreSlug, status, searchTerm, sortColumn, sort, approval, isPublished, pageIndex ?? 1, pageSize ?? 10);
        var rs = await _sender.Send(query);
        return rs.Match(data => Ok(data), Problem);
    }
    [HttpGet("public")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublicManga(
        string? genreSlug,
        MangaStatus? status,
        string? searchTerm = null,
        string? sortColumn = null,
        string? orderBy = null,
        int? pageIndex = 1,
        int? pageSize = 10)
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
        var query = new GetPublicMangaQuery(genreSlug, status, searchTerm, sortColumn, sort, pageIndex ?? 1, pageSize ?? 10);
        var rs = await _sender.Send(query);
        return rs.Match(data => Ok(data), Problem);
    }
    [HttpPut("{id:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [CheckAccess(permission: Permission.Manga.Approve)]
    public async Task<IActionResult> ApproveManga(Guid id)
    {
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Unauthorized();
        }

        var command = new ApproveMangaCommand(id, Guid.Parse(userClaimId.Value));
        var result = await _sender.Send(command);

        return result.Match(_ => NoContent(), Problem);
    }
    [HttpPut("{id:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [CheckAccess(permission: Permission.Manga.Approve)]
    public async Task<IActionResult> RejectManga(Guid id)
    {
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Unauthorized();
        }

        var command = new RejectMangaCommand(id, Guid.Parse(userClaimId.Value));
        var result = await _sender.Send(command);

        return result.Match(_ => NoContent(), Problem);
    }
    [HttpPut("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [CheckAccess(permission: Permission.Manga.Approve)]
    public async Task<IActionResult> PublishManga(Guid id)
    {
        var command = new PublishMangaCommand(id);
        var result = await _sender.Send(command);

        return result.Match(_ => NoContent(), Problem);
    }
    [HttpPut("{id:guid}/un-publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [CheckAccess(permission: Permission.Manga.Approve)]
    public async Task<IActionResult> UnPublishManga(Guid id)
    {
        var command = new UnPublishMangaCommand(id);
        var result = await _sender.Send(command);

        return result.Match(_ => NoContent(), Problem);
    }
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [CheckAccess(permission: Permission.Manga.Update)]
    public async Task<IActionResult> UpdateManga(Guid id,[FromForm] UpdateMangaCommand request)
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
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMangaById(Guid id)
    {
        var query = new GetMangaByIdQuery(id);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data), Problem);
    }
    [HttpGet("{slug}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMangaBySlug(string slug)
    {
        var query = new GetMangaBySlugQuery(slug);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data), Problem);
    }


    [HttpPost("{id:guid}/read")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReadManga(Guid id, [FromBody] ChapterIsRead chapterIsRead)
    {
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Unauthorized();
        }
        var command = new AddReadingHistoryCommand(userId, id, chapterIsRead.ChapterIdRead);
        var result = await _sender.Send(command);

        return result.Match(_ => NoContent(), Problem);
    }
    [HttpGet("history")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReadHistoryManga( int pageIndex =1, int pageSize= 20)
    {
        if (pageIndex <= 0)
        {
            pageIndex = 1;
        }
        if (pageSize <= 0 || pageSize >= 100)
        {
            pageSize = 10;
        }
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Unauthorized();
        }
        var query = new GetHistoryQuery(userId, pageIndex,pageSize);
        var result = await _sender.Send(query);

        return result.Match(data => Ok(data), Problem);
    }

    [HttpDelete("{mangaId:guid}/history")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveMangaHistory(Guid mangaId)
    {
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Unauthorized();
        }
        var command = new RemoveMangaHistoryCommand(userId, mangaId);
        var result = await _sender.Send(command);

        return result.Match(_=> NoContent(), Problem);
    }
    [HttpDelete("history")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveAllMangaHistory()
    {
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Unauthorized();
        }
        var command = new RemoveHistoryCommand(userId);
        var result = await _sender.Send(command);

        return result.Match(_ => NoContent(), Problem);
    }
}
