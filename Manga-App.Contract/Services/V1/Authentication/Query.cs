

using MangaApp.Contract.Abstractions.Messages;
using static MangaApp.Contract.Services.V1.Authentication.Response;

namespace MangaApp.Contract.Services.V1.Authentication;

public static class Query
{
    public record LoginQuery(string UserNameOrEmail, string Password, string Device) : IQuery<LoginResponse>;
    public record RefreshTokenQuery(Guid UserId, string RefreshToken, string Device) : IQuery<RefreshTokenResponse>;
}
