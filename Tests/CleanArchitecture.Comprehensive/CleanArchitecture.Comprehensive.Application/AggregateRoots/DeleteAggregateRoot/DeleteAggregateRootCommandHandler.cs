using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots.DeleteAggregateRoot
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateRootCommandHandler : IRequestHandler<DeleteAggregateRootCommand>
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteAggregateRootCommandHandler(IAggregateRootRepository aggregateRootRepository)
        {
            _aggregateRootRepository = aggregateRootRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteAggregateRootCommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRoot = await _aggregateRootRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAggregateRoot is null)
            {
                throw new NotFoundException($"Could not find AggregateRoot '{request.Id}'");
            }

            _aggregateRootRepository.Remove(existingAggregateRoot);

        }
    }
}