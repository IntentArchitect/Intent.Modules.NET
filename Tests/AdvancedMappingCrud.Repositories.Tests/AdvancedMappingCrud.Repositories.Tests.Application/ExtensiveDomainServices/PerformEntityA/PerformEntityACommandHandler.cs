using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.ExtensiveDomainServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformEntityA
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PerformEntityACommandHandler : IRequestHandler<PerformEntityACommand>
    {
        private readonly IExtensiveDomainService _extensiveDomainService;

        [IntentManaged(Mode.Merge)]
        public PerformEntityACommandHandler(IExtensiveDomainService extensiveDomainService)
        {
            _extensiveDomainService = extensiveDomainService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(PerformEntityACommand request, CancellationToken cancellationToken)
        {
            _extensiveDomainService.PerformEntityA(new ConcreteEntityA
            {
                ConcreteAttr = request.ConcreteAttr,
                BaseAttr = request.BaseAttr
            });
        }
    }
}