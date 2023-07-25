using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using GraphQL.MongoDb.TestApplication.Application.Common.Interfaces;
using GraphQL.MongoDb.TestApplication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.MongoDbUnitOfWorkBehaviour", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Common.Behaviours
{
    public class MongoDbUnitOfWorkBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, ICommand
    {
        private readonly IMongoDbUnitOfWork _dataSource;

        public MongoDbUnitOfWorkBehaviour(IMongoDbUnitOfWork dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next();
            await _dataSource.SaveChangesAsync(cancellationToken);
            return response;
        }
    }
}