using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.UpdateAggregateTestNoIdReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateAggregateTestNoIdReturnCommandHandler : IRequestHandler<UpdateAggregateTestNoIdReturnCommand>
    {
        private readonly IAggregateTestNoIdReturnRepository _aggregateTestNoIdReturnRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateAggregateTestNoIdReturnCommandHandler(IAggregateTestNoIdReturnRepository aggregateTestNoIdReturnRepository)
        {
            _aggregateTestNoIdReturnRepository = aggregateTestNoIdReturnRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateAggregateTestNoIdReturnCommand request, CancellationToken cancellationToken)
        {
            var existingAggregateTestNoIdReturn = await _aggregateTestNoIdReturnRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingAggregateTestNoIdReturn is null)
            {
                throw new NotFoundException($"Could not find AggregateTestNoIdReturn '{request.Id}'");
            }

            existingAggregateTestNoIdReturn.Attribute = request.Attribute;

        }
    }
}