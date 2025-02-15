
using MangaApp.Presentation.Abstraction;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MangaApp.Contract.Services.V1.Authentication.Command;
using static MangaApp.Contract.Services.V1.Authentication.Query;

namespace MangaApp.Presentation.Controllers.V1;
[Route("api/v{version:apiVersion}/auth")] // Version in URL
[ApiVersion("1.0")]

public class AuthenticationController : ApiController
{
    public AuthenticationController(ISender sender) : base(sender)
    {
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginQuery request)
    {
        var result = await _sender.Send(request);
        return result.Match(data => Ok(data), Problem);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterCommand request)
    {
        var result = await _sender.Send(request);
        return result.Match(_ => Ok(new
        {
            Message = "Create new account successfully."
        }), Problem);
    }
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromForm] RefreshTokenQuery request)
    {
        var result = await _sender.Send(request);
        return result.Match(data => Ok(data), Problem);
    }
}
