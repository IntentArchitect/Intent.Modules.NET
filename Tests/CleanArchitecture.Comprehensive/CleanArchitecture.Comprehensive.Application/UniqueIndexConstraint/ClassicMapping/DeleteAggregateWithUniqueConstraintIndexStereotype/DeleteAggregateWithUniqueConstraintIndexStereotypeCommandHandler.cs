using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.DeleteAggregateWithUniqueConstraintIndexStereotype
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateWithUniqueConstraintIndexStereotypeCommandHandler : IRequestHandler<DeleteAggregateWithUniqueConstraintIndexStereotypeCommand>
    {
        private readonly IAggregateWithUniqueConstraintIndexStereotypeRepository _aggregateWithUniqueConstraintIndexStereotypeRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteAggregateWithUniqueConstraintIndexStereotypeCommandHandler(IAggregateWithUniqueConstraintIndexStereotypeRepository aggregateWithUniqueConstraintIndexStereotypeRepository)
        {
            _aggregateWithUniqueConstraintIndexStereotypeRepository = aggregateWithUniqueConstraintIndexStereotypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(
            DeleteAggregateWithUniqueConstraintIndexStereotypeCommand request,
            CancellationToken cancellationToken)
        {
            var existingAggregateWithUniqueConstraintIndexStereotype = await _aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAggregateWithUniqueConstraintIndexStereotype is null)
            {
                throw new NotFoundException($"Could not find AggregateWithUniqueConstraintIndexStereotype '{request.Id}'");
            }

            _aggregateWithUniqueConstraintIndexStereotypeRepository.Remove(existingAggregateWithUniqueConstraintIndexStereotype);
        }
    }
}