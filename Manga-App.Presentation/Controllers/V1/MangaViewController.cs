
using MangaApp.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MangaApp.Contract.Services.V1.MangaView.Command;
using static MangaApp.Contract.Services.V1.MangaView.Query;

namespace MangaApp.Presentation.Controllers.V1;
[Route("api/v{version:apiVersion}/manga")] // Version in URL
[ApiVersion("1.0")]
public class MangaViewController : ApiController
{
    public MangaViewController(ISender sender) : base(sender)
    {
    }
    [HttpPost("{mangaId:guid}/view")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> IncreaseMangaView(Guid mangaId)
    {
        var command = new IncreaseMangaViewCommand(mangaId);
        var result = await _sender.Send(command);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncMangaViews()
    {
        var result = await _sender.Send(new SyncMangaViewCommand());
        return Ok("View count synchronized.");
    }
    [HttpGet("{mangaId}/view")]
    public async Task<IActionResult> GetMangaView(Guid mangaId)
    {
        var result = await _sender.Send(new GetViewMangaQuery(mangaId));
        return Ok("View count synchronized.");
    }
}
