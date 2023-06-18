using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.CosmosDB.CosmosDBUnitOfWorkBehaviour", Version = "1.0")]

namespace CosmosDB.Application.Common.Behaviours
{
    public class CosmosDBUnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICommand
    {
        private readonly ICosmosDBUnitOfWork _cosmosDBUnitOfWork;

        public CosmosDBUnitOfWorkBehaviour(ICosmosDBUnitOfWork cosmosDBUnitOfWork)
        {
            _cosmosDBUnitOfWork = cosmosDBUnitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            await _cosmosDBUnitOfWork.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}