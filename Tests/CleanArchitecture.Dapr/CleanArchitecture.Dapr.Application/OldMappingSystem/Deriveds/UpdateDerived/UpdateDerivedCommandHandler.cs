using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.UpdateDerived
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateDerivedCommandHandler : IRequestHandler<UpdateDerivedCommand>
    {
        private readonly IDerivedRepository _derivedRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateDerivedCommandHandler(IDerivedRepository derivedRepository)
        {
            _derivedRepository = derivedRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateDerivedCommand request, CancellationToken cancellationToken)
        {
            var existingDerived = await _derivedRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingDerived is null)
            {
                throw new NotFoundException($"Could not find Derived '{request.Id}'");
            }

            existingDerived.Attribute = request.Attribute;
            existingDerived.BaseAttribute = request.BaseAttribute;

            _derivedRepository.Update(existingDerived);

        }
    }
}