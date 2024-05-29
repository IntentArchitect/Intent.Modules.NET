using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.AdvancedMapping.UpdateAdvAggregateWithUniqueConstraintIndexElement
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAdvAggregateWithUniqueConstraintIndexElementCommandHandler : IRequestHandler<UpdateAdvAggregateWithUniqueConstraintIndexElementCommand>
    {
        private readonly IAggregateWithUniqueConstraintIndexElementRepository _aggregateWithUniqueConstraintIndexElementRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAdvAggregateWithUniqueConstraintIndexElementCommandHandler(IAggregateWithUniqueConstraintIndexElementRepository aggregateWithUniqueConstraintIndexElementRepository)
        {
            _aggregateWithUniqueConstraintIndexElementRepository = aggregateWithUniqueConstraintIndexElementRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            UpdateAdvAggregateWithUniqueConstraintIndexElementCommand request,
            CancellationToken cancellationToken)
        {
            var aggregateWithUniqueConstraintIndexElement = await _aggregateWithUniqueConstraintIndexElementRepository.FindByIdAsync(request.Id, cancellationToken);
            if (aggregateWithUniqueConstraintIndexElement is null)
            {
                throw new NotFoundException($"Could not find AggregateWithUniqueConstraintIndexElement '{request.Id}'");
            }

            aggregateWithUniqueConstraintIndexElement.SingleUniqueField = request.SingleUniqueField;
            aggregateWithUniqueConstraintIndexElement.CompUniqueFieldA = request.CompUniqueFieldA;
            aggregateWithUniqueConstraintIndexElement.CompUniqueFieldB = request.CompUniqueFieldB;
        }
    }
}