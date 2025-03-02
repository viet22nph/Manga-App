using MangaApp.Contract.Dtos.Rating;
using MangaApp.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MangaApp.Contract.Services.V1.Rating.Command;

namespace MangaApp.Presentation.Controllers.V1;

[Route("api/v{version:apiVersion}/rating")] // Version in URL
[ApiVersion("1.0")]
public class RatingController : ApiController
{
    public RatingController(ISender sender) : base(sender)
    {
    }


    [HttpPost("{mangaId:guid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrUpdateRatingManga(Guid mangaId,[FromBody] RatingDto rating)
    {
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Forbid();
        }
        var command = new CreateOrUpdateMangaRatingCommand(userId, mangaId, rating.Rating);
        var result = await _sender.Send(command);
        return result.Match(_ => Ok(new
        {
            result = "ok"
        }), Problem);
    }
    [HttpDelete("{mangaId:guid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRatingManga(Guid mangaId)
    {
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Forbid();
        }
        var command = new DeleteMangaRatingCommand(userId, mangaId);
        var result = await _sender.Send(command);
        return result.Match(_ => Ok(new
        {
            result = "ok"
        }), Problem);
    }
}
