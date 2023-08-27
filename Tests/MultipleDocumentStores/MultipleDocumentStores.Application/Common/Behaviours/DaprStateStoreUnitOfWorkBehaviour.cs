using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MultipleDocumentStores.Application.Common.Interfaces;
using MultipleDocumentStores.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreUnitOfWorkBehaviour", Version = "1.0")]

namespace MultipleDocumentStores.Application.Common.Behaviours
{
    public class DaprStateStoreUnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>, ICommand
        where TRequest : notnull
    {
        private readonly IDaprStateStoreUnitOfWork _daprStateStoreUnitOfWork;

        public DaprStateStoreUnitOfWorkBehaviour(IDaprStateStoreUnitOfWork daprStateStoreUnitOfWork)
        {
            _daprStateStoreUnitOfWork = daprStateStoreUnitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next();

            await _daprStateStoreUnitOfWork.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}