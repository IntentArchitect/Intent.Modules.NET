using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using AdvancedMappingCrudMongo.Tests.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Common.Behaviours
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
        private readonly IMongoDbUnitOfWork _mongoDbDataSource;

        public UnitOfWorkBehaviour(IMongoDbUnitOfWork mongoDbDataSource)
        {
            _mongoDbDataSource = mongoDbDataSource ?? throw new ArgumentNullException(nameof(mongoDbDataSource));
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next();
            await _mongoDbDataSource.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}