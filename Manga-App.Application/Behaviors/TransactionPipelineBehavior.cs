

using MangApp.Application.Abstraction;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MangaApp.Application.Behaviors;

public sealed class TransactionPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{

    private readonly IUnitOfWork _unitOfWork; // SQL-SERVER-STRATEGY-1
    public TransactionPipelineBehavior(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!IsCommand()) // In case TRequest is QueryRequest just ignore
            return await next();
        var strategy = _unitOfWork.GetDbContext().Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _unitOfWork.GetDbContext().Database.BeginTransactionAsync();
            {
                var response = await next();
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
                return response;
            }
        });
    }
    private bool IsCommand()
          => typeof(TRequest).Name.EndsWith("Command");
}
