using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpointsTest.Domain.Common.Exceptions;
using FastEndpointsTest.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FastEndpointsTest.Application.AggregateWithUniqueConstraintIndexStereotypes.UpdateAggregateWithUniqueConstraintIndexStereotype
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateWithUniqueConstraintIndexStereotypeCommandHandler : IRequestHandler<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>
    {
        private readonly IAggregateWithUniqueConstraintIndexStereotypeRepository _aggregateWithUniqueConstraintIndexStereotypeRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAggregateWithUniqueConstraintIndexStereotypeCommandHandler(IAggregateWithUniqueConstraintIndexStereotypeRepository aggregateWithUniqueConstraintIndexStereotypeRepository)
        {
            _aggregateWithUniqueConstraintIndexStereotypeRepository = aggregateWithUniqueConstraintIndexStereotypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            UpdateAggregateWithUniqueConstraintIndexStereotypeCommand request,
            CancellationToken cancellationToken)
        {
            var aggregateWithUniqueConstraintIndexStereotype = await _aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (aggregateWithUniqueConstraintIndexStereotype is null)
            {
                throw new NotFoundException($"Could not find AggregateWithUniqueConstraintIndexStereotype '{request.Id}'");
            }

            aggregateWithUniqueConstraintIndexStereotype.SingleUniqueField = request.SingleUniqueField;
            aggregateWithUniqueConstraintIndexStereotype.CompUniqueFieldA = request.CompUniqueFieldA;
            aggregateWithUniqueConstraintIndexStereotype.CompUniqueFieldB = request.CompUniqueFieldB;
        }
    }
}