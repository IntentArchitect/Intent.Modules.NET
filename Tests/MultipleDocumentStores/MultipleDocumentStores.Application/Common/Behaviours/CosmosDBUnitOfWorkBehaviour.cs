using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MultipleDocumentStores.Application.Common.Interfaces;
using MultipleDocumentStores.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBUnitOfWorkBehaviour", Version = "1.0")]

namespace MultipleDocumentStores.Application.Common.Behaviours
{
    public class CosmosDBUnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, ICommand
    {
        private readonly ICosmosDBUnitOfWork _cosmosDBUnitOfWork;

        public CosmosDBUnitOfWorkBehaviour(ICosmosDBUnitOfWork cosmosDBUnitOfWork)
        {
            _cosmosDBUnitOfWork = cosmosDBUnitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next();

            await _cosmosDBUnitOfWork.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}