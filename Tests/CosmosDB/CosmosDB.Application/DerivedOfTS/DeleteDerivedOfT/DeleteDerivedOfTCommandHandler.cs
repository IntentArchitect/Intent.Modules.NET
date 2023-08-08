using System;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.DerivedOfTS.DeleteDerivedOfT
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDerivedOfTCommandHandler : IRequestHandler<DeleteDerivedOfTCommand>
    {
        private readonly IDerivedOfTRepository _derivedOfTRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDerivedOfTCommandHandler(IDerivedOfTRepository derivedOfTRepository)
        {
            _derivedOfTRepository = derivedOfTRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDerivedOfTCommand request, CancellationToken cancellationToken)
        {
            var existingDerivedOfT = await _derivedOfTRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingDerivedOfT is null)
            {
                throw new NotFoundException($"Could not find DerivedOfT '{request.Id}'");
            }

            _derivedOfTRepository.Remove(existingDerivedOfT);

        }
    }
}