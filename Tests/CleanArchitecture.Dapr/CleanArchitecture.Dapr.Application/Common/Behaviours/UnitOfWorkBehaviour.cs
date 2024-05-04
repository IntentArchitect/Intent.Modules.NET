using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Application.Common.Interfaces;
using CleanArchitecture.Dapr.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Common.Behaviours
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
        private readonly IDaprStateStoreUnitOfWork _daprStateStoreDataSource;

        public UnitOfWorkBehaviour(IDaprStateStoreUnitOfWork daprStateStoreDataSource)
        {
            _daprStateStoreDataSource = daprStateStoreDataSource ?? throw new ArgumentNullException(nameof(daprStateStoreDataSource));
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next();

            await _daprStateStoreDataSource.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}