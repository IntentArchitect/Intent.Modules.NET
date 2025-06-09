using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using MinimalHostingModel.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.Behaviours.UnitOfWorkBehaviour", Version = "1.0")]

namespace MinimalHostingModel.Application.Common.Behaviours
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
        private readonly IDistributedCacheWithUnitOfWork _distributedCacheWithUnitOfWork;

        public UnitOfWorkBehaviour(IDistributedCacheWithUnitOfWork distributedCacheWithUnitOfWork)
        {
            _distributedCacheWithUnitOfWork = distributedCacheWithUnitOfWork ?? throw new ArgumentNullException(nameof(distributedCacheWithUnitOfWork));
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                var response = await next(cancellationToken);
                await _distributedCacheWithUnitOfWork.SaveChangesAsync(cancellationToken);

                return response;
            }
        }
    }
}