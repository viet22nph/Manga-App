
using MangaApp.Contract.Abstractions.Messages;

namespace MangaApp.Contract.Services.V1.Authentication;

public static class Event
{
    public record RefreshTokenedEvent(Guid UserId, string RefreshToken, string Device) : IEvent;
}
