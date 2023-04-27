using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Subscribe.CleanArchDapr.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.StateRepositoryUnitOfWorkBehaviour", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Application.Common.Behaviours
{
    public class StateRepositoryUnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IStateRepository _stateRepository;

        public StateRepositoryUnitOfWorkBehaviour(IStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            await _stateRepository.FlushAllAsync(cancellationToken);

            return response;
        }
    }
}