using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreUnitOfWorkBehaviour", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.Common.Behaviours
{
    public class DaprStateStoreUnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IDaprStateStoreUnitOfWork _daprStateStoreUnitOfWork;

        public DaprStateStoreUnitOfWorkBehaviour(IDaprStateStoreUnitOfWork daprStateStoreUnitOfWork)
        {
            _daprStateStoreUnitOfWork = daprStateStoreUnitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            await _daprStateStoreUnitOfWork.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}