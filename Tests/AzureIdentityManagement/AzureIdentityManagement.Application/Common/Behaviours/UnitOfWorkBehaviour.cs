using AzureIdentityManagement.Application.Common.Interfaces;
using AzureIdentityManagement.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour", Version = "1.0")]

namespace AzureIdentityManagement.Application.Common.Behaviours
{
    /// <summary>
    /// Ensures that all operations processed as part of handling a <see cref="ICommand"/> either
    /// pass or fail as one unit. This behaviour makes it unnecessary for developers to call
    /// SaveChangesAsync() inside their business logic (e.g. command handlers), and doing so should
    /// be avoided unless absolutely necessary.
    /// </summary>
    public class UnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, ICommand
    {
        private readonly ICosmosDBUnitOfWork _cosmosDBDataSource;
        private readonly ITableStorageUnitOfWork _tableStorageDataSource;

        public UnitOfWorkBehaviour(ICosmosDBUnitOfWork cosmosDBDataSource, ITableStorageUnitOfWork tableStorageDataSource)
        {
            _cosmosDBDataSource = cosmosDBDataSource ?? throw new ArgumentNullException(nameof(cosmosDBDataSource));
            _tableStorageDataSource = tableStorageDataSource ?? throw new ArgumentNullException(nameof(tableStorageDataSource));
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken);
            await _tableStorageDataSource.SaveChangesAsync(cancellationToken);
            await _cosmosDBDataSource.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}