using DynamoDbTests.EnumAsStrings.Application.Common.Interfaces;
using DynamoDbTests.EnumAsStrings.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour", Version = "1.0")]

namespace DynamoDbTests.EnumAsStrings.Application.Common.Behaviours
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
        private readonly IDynamoDBUnitOfWork _dynamoDBUnitOfWork;

        public UnitOfWorkBehaviour(IDynamoDBUnitOfWork dynamoDBUnitOfWork)
        {
            _dynamoDBUnitOfWork = dynamoDBUnitOfWork ?? throw new ArgumentNullException(nameof(dynamoDBUnitOfWork));
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var response = await next(cancellationToken);
            await _dynamoDBUnitOfWork.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}