

using MangaApp.Contract.Shares;
using MediatR;

namespace MangaApp.Contract.Abstractions.Messages;

/// <summary>
/// Represents a handler for processing commands of type <typeparamref name="TRequest"/> 
/// and returning a response of type <typeparamref name="TResponse"/>.
/// This interface is used to define the logic for handling commands within the application,
/// and it inherits from <see cref="IRequestHandler{TRequest, Result{TResponse}}"/> 
/// to integrate with MediatR.
/// </summary>
/// <typeparam name="TRequest">The type of the command request that implements <see cref="ICommand{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the command handler.</typeparam>
/// <remarks>
/// Command handlers are responsible for executing the business logic associated with a command,
/// processing the request, and returning a standardized result wrapped in <see cref="Result{TResponse}"/>.
/// This allows for consistent handling of success and error states across the application.
/// </remarks>
public interface ICommandHandler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
    where TRequest : ICommand<TResponse>
{
}