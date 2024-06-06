using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs.DeleteAggregateRootLong
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateRootLongCommandHandler : IRequestHandler<DeleteAggregateRootLongCommand>
    {
        private readonly IAggregateRootLongRepository _aggregateRootLongRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteAggregateRootLongCommandHandler(IAggregateRootLongRepository aggregateRootLongRepository)
        {
            _aggregateRootLongRepository = aggregateRootLongRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteAggregateRootLongCommand request, CancellationToken cancellationToken)
        {
            var existingAggregateRootLong = await _aggregateRootLongRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAggregateRootLong is null)
            {
                throw new NotFoundException($"Could not find AggregateRootLong '{request.Id}'");
            }

            _aggregateRootLongRepository.Remove(existingAggregateRootLong);

        }
    }
}