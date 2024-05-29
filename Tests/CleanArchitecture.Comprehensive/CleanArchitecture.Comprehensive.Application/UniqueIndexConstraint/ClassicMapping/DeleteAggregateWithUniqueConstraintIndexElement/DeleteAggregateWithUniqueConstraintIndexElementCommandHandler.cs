using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.DeleteAggregateWithUniqueConstraintIndexElement
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateWithUniqueConstraintIndexElementCommandHandler : IRequestHandler<DeleteAggregateWithUniqueConstraintIndexElementCommand>
    {
        private readonly IAggregateWithUniqueConstraintIndexElementRepository _aggregateWithUniqueConstraintIndexElementRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteAggregateWithUniqueConstraintIndexElementCommandHandler(IAggregateWithUniqueConstraintIndexElementRepository aggregateWithUniqueConstraintIndexElementRepository)
        {
            _aggregateWithUniqueConstraintIndexElementRepository = aggregateWithUniqueConstraintIndexElementRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            DeleteAggregateWithUniqueConstraintIndexElementCommand request,
            CancellationToken cancellationToken)
        {
            var existingAggregateWithUniqueConstraintIndexElement = await _aggregateWithUniqueConstraintIndexElementRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAggregateWithUniqueConstraintIndexElement is null)
            {
                throw new NotFoundException($"Could not find AggregateWithUniqueConstraintIndexElement '{request.Id}'");
            }

            _aggregateWithUniqueConstraintIndexElementRepository.Remove(existingAggregateWithUniqueConstraintIndexElement);
        }
    }
}