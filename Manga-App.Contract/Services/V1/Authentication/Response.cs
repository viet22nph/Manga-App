using MangaApp.Contract.Dtos.User;

namespace MangaApp.Contract.Services.V1.Authentication;

public static class Response
{
    public record LoginResponse(string AccessToken, string RefreshToken);
    public record RefreshTokenResponse(string AccessToken, string RefreshToken);
}
