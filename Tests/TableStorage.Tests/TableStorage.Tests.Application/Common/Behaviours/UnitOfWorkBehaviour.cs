using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Interfaces;
using TableStorage.Tests.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour", Version = "1.0")]

namespace TableStorage.Tests.Application.Common.Behaviours
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
        private readonly ITableStorageUnitOfWork _tableStorageDataSource;

        public UnitOfWorkBehaviour(ITableStorageUnitOfWork tableStorageDataSource)
        {
            _tableStorageDataSource = tableStorageDataSource ?? throw new ArgumentNullException(nameof(tableStorageDataSource));
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken);

            await _tableStorageDataSource.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}