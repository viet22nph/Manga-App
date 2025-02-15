
using MangaApp.Application.Abstraction.Services;
using MangaApp.Application.Abstraction.Services.CacheService;
using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Services.V1.Authentication;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Constants;
using MangaApp.Contract.Shares.Errors;
using MangaApp.Contract.Shares.Helper;
using MangaApp.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static MangaApp.Contract.Services.V1.Authentication.Query;
using static MangaApp.Contract.Services.V1.Authentication.Response;

namespace MangaApp.Application.UserCases.V1.Queries.Authentication;

public class GetRefreshTokenQueryHandler : IQueryHandler<RefreshTokenQuery, RefreshTokenResponse>
{

    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICacheManager _cacheManager;
    private readonly IPublisher _publisher;
    public GetRefreshTokenQueryHandler(
        UserManager<AppUser> userManager, 
        IJwtTokenService jwtTokenService,
        ICacheManager cacheManager,
        IPublisher publisher
        )
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _cacheManager = cacheManager;
        _publisher = publisher;
    }
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return Error.NotFound(code: nameof(request.UserId), description: "User id not found");
        }
        // check refresh token exists in redis
        string key = $"{RedisKey.LIST_REFRESH_KEY}{request.UserId}-{request.Device}";
        string refreshToken = await _cacheManager.GetAsync(key);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Error.Unauthorized(code: nameof(request.RefreshToken), description: "The refresh token has expired.Please log in again");
        }
        // check refersh 
        if (refreshToken.Trim('"') != request.RefreshToken)
        {
            // remove refresh token in redis
            _cacheManager.Remove(key);
            return Error.Unauthorized(code: nameof(request.RefreshToken), description: "The refresh token is invalid or does not match the one stored. Please log in again.");
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
        // overwrite refresh token in redis
        await _publisher.Publish(new Event.RefreshTokenedEvent(user.Id, generateRefreshToken, request.Device));
        var refreshTokenResponse = new Response.RefreshTokenResponse(generateAccessToken, generateRefreshToken);
        return refreshTokenResponse;
    }
}
