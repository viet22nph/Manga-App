

using MangaApp.Application.Abstraction.Services;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Dtos.User;
using MangaApp.Contract.Shares;
using MangaApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static MangaApp.Contract.Services.V1.Authentication.Query;
using static MangaApp.Contract.Services.V1.Authentication.Response;
using System.Security.Claims;
using MangaApp.Contract.Shares.Errors;
using System.IdentityModel.Tokens.Jwt;
using MangaApp.Contract.Shares.Helper;
using System.Linq;
using MangaApp.Contract.Services.V1.Authentication;

namespace MangaApp.Application.UserCases.V1.Queries.Authentication;

public class GetLoginQueryHandler : IQueryHandler<LoginQuery, LoginResponse>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    private readonly IPublisher _publisher;
    public GetLoginQueryHandler(UserManager<AppUser> userManager, IJwtTokenService jwtTokenService, IPublisher publisher)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _publisher = publisher;
    }

    public async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        // check user
        var user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
        if (user == null)
        {
            user = await _userManager.FindByNameAsync(request.UserNameOrEmail);
            if (user == null)
            {
                return Error.Validation(code: nameof(request.UserNameOrEmail), description: $"You are not registered with '{request.UserNameOrEmail}'.");
            }
        }


        bool signInResult = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!signInResult)
        {
            return Error.Validation(code: nameof(request.UserNameOrEmail), description: $"Invalid credentials for '{request.UserNameOrEmail}'.");
        }
        string ip = Helper.GetIpAddress();
        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("uid", user.Id.ToString()),
            new Claim("ip", ip),
            new Claim("Role", string.Join(", ", roles))
        };
        string generateAccessToken = _jwtTokenService.GenerateAccessToken(claims);
        string generateRefreshToken = _jwtTokenService.GenerateRefreshToken();
        
        var loginResponse = new LoginResponse(generateAccessToken, generateRefreshToken);
        //call event save refresh token
        await _publisher.Publish(new Event.RefreshTokenedEvent(user.Id, generateRefreshToken, request.Device));
        return loginResponse;
    }
}