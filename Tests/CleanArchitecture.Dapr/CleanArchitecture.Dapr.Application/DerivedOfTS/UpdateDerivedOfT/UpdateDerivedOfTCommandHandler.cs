using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.DerivedOfTS.UpdateDerivedOfT
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
        public async Task<Unit> Handle(UpdateDerivedOfTCommand request, CancellationToken cancellationToken)
        {
            var existingDerivedOfT = await _derivedOfTRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingDerivedOfT is null)
            {
                throw new NotFoundException($"Could not find DerivedOfT '{request.Id}'");
            }
            existingDerivedOfT.DerivedAttribute = request.DerivedAttribute;
            existingDerivedOfT.GenericTypeAttribute = request.GenericTypeAttribute;

            _derivedOfTRepository.Update(existingDerivedOfT);
            return Unit.Value;
        }
    }
}