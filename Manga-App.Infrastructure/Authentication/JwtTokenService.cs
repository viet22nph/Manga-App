
using MangaApp.Application.Abstraction.Services;
using MangaApp.Infrastructure.DependencyInjection.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MangaApp.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{

    private readonly JwtOptions _jwtOption;

    public JwtTokenService(IOptions<JwtOptions> option)
    {
        _jwtOption = option.Value ?? throw new ArgumentNullException(nameof(option));

        if (string.IsNullOrEmpty(_jwtOption.SecretKey))
        {
            throw new Exception("Jwt key string is empty");
        }
    }
    public ClaimsPrincipal GetClaimsPrincipalFromExpriedToken(string token)
    {
        var key = Encoding.UTF8.GetBytes(_jwtOption.SecretKey);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // on production is true
            ValidateIssuer = false, // on production is true
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtOption.Issuer,
            ValidAudience = _jwtOption.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null)
        {
            throw new SecurityTokenException("Invalid token!");
        }
        return principal;
    }
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtOption.Issuer,
            audience: _jwtOption.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOption.ExpireAccess),
            signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.SecretKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtOption.Issuer,
            audience: _jwtOption.Audience,
            expires: DateTime.UtcNow.AddMinutes(_jwtOption.ExpireRefresh),
            signingCredentials: signingCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        return tokenString;
    }
}