
using System.Security.Claims;

namespace MangaApp.Application.Abstraction.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetClaimsPrincipalFromExpriedToken(string token);
}
