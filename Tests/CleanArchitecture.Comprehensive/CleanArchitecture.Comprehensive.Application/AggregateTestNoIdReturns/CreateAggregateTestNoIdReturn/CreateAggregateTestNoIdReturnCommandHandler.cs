using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.CreateAggregateTestNoIdReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateAggregateTestNoIdReturnCommandHandler : IRequestHandler<CreateAggregateTestNoIdReturnCommand>
    {
        private readonly IAggregateTestNoIdReturnRepository _aggregateTestNoIdReturnRepository;

        [IntentManaged(Mode.Merge)]
        public CreateAggregateTestNoIdReturnCommandHandler(IAggregateTestNoIdReturnRepository aggregateTestNoIdReturnRepository)
        {
            _aggregateTestNoIdReturnRepository = aggregateTestNoIdReturnRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateAggregateTestNoIdReturnCommand request, CancellationToken cancellationToken)
        {
            var newAggregateTestNoIdReturn = new AggregateTestNoIdReturn
            {
                Attribute = request.Attribute,
            };

            _aggregateTestNoIdReturnRepository.Add(newAggregateTestNoIdReturn);

        }
    }
}