
using MangaApp.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MangaApp.Contract.Services.V1.Follow.Query;

namespace MangaApp.Presentation.Controllers.V1;
[Route("api/v{version:apiVersion}/user/follows")] // Version in URL
[ApiVersion("1.0")]
public class FollowController : ApiController
{
    public FollowController(ISender sender) : base(sender)
    {
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpGet("manga")]
    public async Task<IActionResult> GetFollowdManga(int pageIndex =1, int pageSize=10)
    {
        if (pageIndex <= 0)
        {
            return BadRequest(new
            {
                Message = "Page index must be greater than 0",
                ErrorCode = "INVALID_PAGE_INDEX"
            });
        }
        if (pageSize <= 0 || pageSize > 100)
        {
            return BadRequest(new
            {
                Message = "Page size must be between 1 and 100",
                ErrorCode = "INVALID_PAGE_SIZE"
            });
        }
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetFollowedMangaQuery(userId, pageIndex, pageSize);
        var result = await _sender.Send(query);
        return result.Match(data => Ok(data), Problem);
    }
}
