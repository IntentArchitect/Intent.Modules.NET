using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.Application.DerivedOfTS.UpdateDerivedOfT
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDerivedOfTCommandHandler : IRequestHandler<UpdateDerivedOfTCommand>
    {
        private readonly IDerivedOfTRepository _derivedOfTRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateDerivedOfTCommandHandler(IDerivedOfTRepository derivedOfTRepository)
        {
            _derivedOfTRepository = derivedOfTRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateDerivedOfTCommand request, CancellationToken cancellationToken)
        {
            var existingDerivedOfT = await _derivedOfTRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingDerivedOfT is null)
            {
                throw new NotFoundException($"Could not find DerivedOfT '{request.Id}'");
            }

            existingDerivedOfT.DerivedAttribute = request.DerivedAttribute;
            existingDerivedOfT.GenericAttribute = request.GenericAttribute;

            _derivedOfTRepository.Update(existingDerivedOfT);

        }
    }
}