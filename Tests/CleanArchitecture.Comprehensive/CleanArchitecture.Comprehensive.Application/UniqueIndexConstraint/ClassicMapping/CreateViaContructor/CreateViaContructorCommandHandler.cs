using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.CreateViaContructor
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateViaContructorCommandHandler : IRequestHandler<CreateViaContructorCommand>
    {
        private readonly IAggregateWithUniqueConstraintIndexElementRepository _aggregateWithUniqueConstraintIndexElementRepository;

        [IntentManaged(Mode.Merge)]
        public CreateViaContructorCommandHandler(IAggregateWithUniqueConstraintIndexElementRepository aggregateWithUniqueConstraintIndexElementRepository)
        {
            _aggregateWithUniqueConstraintIndexElementRepository = aggregateWithUniqueConstraintIndexElementRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateViaContructorCommand request, CancellationToken cancellationToken)
        {
            var newAggregateWithUniqueConstraintIndexElement = new AggregateWithUniqueConstraintIndexElement(request.SingleUniqueField, request.CompUniqueFieldA, request.CompUniqueFieldB);

            _aggregateWithUniqueConstraintIndexElementRepository.Add(newAggregateWithUniqueConstraintIndexElement);
        }
    }
}