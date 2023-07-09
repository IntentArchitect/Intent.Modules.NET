using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateTestNoIdReturns.DeleteAggregateTestNoIdReturn
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAggregateTestNoIdReturnCommandHandler : IRequestHandler<DeleteAggregateTestNoIdReturnCommand>
    {
        private readonly IAggregateTestNoIdReturnRepository _aggregateTestNoIdReturnRepository;

        [IntentManaged(Mode.Ignore)]
        public DeleteAggregateTestNoIdReturnCommandHandler(IAggregateTestNoIdReturnRepository aggregateTestNoIdReturnRepository)
        {
            _aggregateTestNoIdReturnRepository = aggregateTestNoIdReturnRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Unit> Handle(DeleteAggregateTestNoIdReturnCommand request, CancellationToken cancellationToken)
        {
            var existingAggregateTestNoIdReturn = await _aggregateTestNoIdReturnRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingAggregateTestNoIdReturn is null)
            {
                throw new NotFoundException($"Could not find AggregateTestNoIdReturn '{request.Id}' ");
            }
            _aggregateTestNoIdReturnRepository.Remove(existingAggregateTestNoIdReturn);
            return Unit.Value;
        }
    }
}