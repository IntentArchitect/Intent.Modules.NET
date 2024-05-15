using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Entities;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Deriveds.CreateDerived
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDerivedCommandHandler : IRequestHandler<CreateDerivedCommand, string>
    {
        private readonly IDerivedRepository _derivedRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDerivedCommandHandler(IDerivedRepository derivedRepository)
        {
            _derivedRepository = derivedRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateDerivedCommand request, CancellationToken cancellationToken)
        {
            var newDerived = new Derived
            {
                Attribute = request.Attribute,
                BaseAttribute = request.BaseAttribute,
            };

            _derivedRepository.Add(newDerived);
            await _derivedRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newDerived.Id;
        }
    }
}