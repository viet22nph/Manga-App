

using MangaApp.Contract.Abstractions.Messages;
using MangaApp.Contract.Shares;
using MangaApp.Contract.Shares.Errors;
using MangaApp.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using static MangaApp.Contract.Services.V1.Authentication.Command;

namespace MangaApp.Application.UserCases.V1.Commands.Authentication;



public sealed class RegisterCommandHandler(UserManager<AppUser> _userManager) : ICommandHandler<RegisterCommand, Success>
{
    public async Task<Result<Success>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // check email exists in database
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user != null)
        {
            return Error.Validation(code: nameof(request.Email), description: $"Email {request.Email} is already registered.");
        }
        user = await _userManager.FindByNameAsync(request.UserName);
        if (user != null)
        {
            return Error.Validation(code: nameof(request.Email), description: $"UserName {request.UserName} is already registered.");
        }

        AppUser newUser = AppUser.CreateNewUser(request.Email, request.UserName);
        var result = await _userManager.CreateAsync(newUser, request.Password);
        // add failed 
        if (!result.Succeeded)
        {
            return Error.Failure(result.Errors.First().Code, result.Errors.First().Description);
        }
        // add role client 
        await _userManager.AddToRoleAsync(newUser, "User");
        return ResultType.Success;
    }
}
