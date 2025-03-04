

using MangaApp.Application.UserCases.V1.Queries.Comment;
using MangaApp.Contract.Dtos.Comment;
using MangaApp.Contract.Shares.Enums;
using MangaApp.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MangaApp.Contract.Services.V1.Comment.Command;
using static MangaApp.Contract.Services.V1.Comment.Query;

namespace MangaApp.Presentation.Controllers.V1;
[Route("api/v{version:apiVersion}/comment")] // Version in URL
[ApiVersion("1.0")]
public class CommentController : ApiController
{
    public CommentController(ISender sender) : base(sender)
    {
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateComment([FromBody] CommentRequestDto request)
    {
        var userClaimId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "uid");
        if (userClaimId == null || !Guid.TryParse(userClaimId.Value, out var userId))
        {
            return Unauthorized();
        }
        var command = new CreateCommentCommand(request.Content, userId, request.CommentType, request.TypeId, request.ParentId);
        var result = await _sender.Send(command);
        return result.Match(_ => Created(), Problem);
    }

    [HttpGet]
    public async Task<IActionResult> GetComment([FromQuery] CommentType type,
    [FromQuery] Guid targetId,
    [FromQuery] Guid? parentId,
    [FromQuery] int pageIndex = 1,
    [FromQuery] int pageSize = 15)
    {
        //lấy comment
        if(parentId == null)
        {

            var queryComment = new GetCommentQuery(type, targetId, pageIndex, pageSize);
            var resultComment = await _sender.Send(queryComment);
            return resultComment.Match(data => Ok(data), Problem);
        }    

        // lấy reply

        var queryReply = new GetRepliesQuery(type, targetId,parentId.Value, pageIndex, pageSize);
        var resultReply = await _sender.Send(queryReply);
        return resultReply.Match(data => Ok(data), Problem);

    }
   
}
