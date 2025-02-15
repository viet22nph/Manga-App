using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;

namespace MangaApp.Contract.Services.V1.Authentication;

public static class Command
{
    public record RegisterCommand(string Email, string UserName, string Password, string ConfirmPassword) : ICommand<Success>;
}
