using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using EntityFrameworkCore.MultiConnectionStrings.Application.Common.Interfaces;
using EntityFrameworkCore.MultiConnectionStrings.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.MultiConnectionStrings.Infrastructure;

public class AlternateDbUnitOfWork<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, ICommand
{
    private readonly AlternateDbDbContext _dbContext;

    public AlternateDbUnitOfWork(AlternateDbDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (Transaction.Current is not null)
        {
            await _dbContext.Database.OpenConnectionAsync(cancellationToken);
            _dbContext.Database.EnlistTransaction(Transaction.Current);
            
            var response = await next();

            await _dbContext.SaveChangesAsync(cancellationToken);
            return response;
        }
        
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next();

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            
            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}