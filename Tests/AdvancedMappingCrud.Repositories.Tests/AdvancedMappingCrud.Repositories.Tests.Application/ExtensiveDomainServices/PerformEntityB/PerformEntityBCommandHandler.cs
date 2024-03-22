using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.ExtensiveDomainServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformEntityB
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PerformEntityBCommandHandler : IRequestHandler<PerformEntityBCommand>
    {
        private readonly IExtensiveDomainService _extensiveDomainService;

        [IntentManaged(Mode.Merge)]
        public PerformEntityBCommandHandler(IExtensiveDomainService extensiveDomainService)
        {
            _extensiveDomainService = extensiveDomainService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(PerformEntityBCommand request, CancellationToken cancellationToken)
        {
            _extensiveDomainService.PerformEntityB(new ConcreteEntityB(
    baseAttr: request.BaseAttr,
    concreteAttr: request.ConcreteAttr));
        }
    }
}