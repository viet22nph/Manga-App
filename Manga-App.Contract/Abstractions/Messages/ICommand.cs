
using MangaApp.Contract.Shares;
using MediatR;

namespace MangaApp.Contract.Abstractions.Messages;
/// <summary>
/// Represents a command that returns a response of type <typeparamref name="TResponse"/>.
/// This interface is used to define commands that can be executed within the application,
/// and it inherits from <see cref="IRequest{Result{TResponse}}"/> to integrate with MediatR.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
/// <remarks>
/// Commands are typically used to perform actions that modify the state of the application.
/// The <see cref="Result{TResponse}"/> wrapper is used to standardize the response format,
/// including success/failure status and any associated messages or errors.
/// </remarks>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
