using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.ExtensiveDomainServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformPassthroughAsync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PerformPassthroughAsyncCommandHandler : IRequestHandler<PerformPassthroughAsyncCommand>
    {
        private readonly IExtensiveDomainService _extensiveDomainService;

        [IntentManaged(Mode.Merge)]
        public PerformPassthroughAsyncCommandHandler(IExtensiveDomainService extensiveDomainService)
        {
            _extensiveDomainService = extensiveDomainService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(PerformPassthroughAsyncCommand request, CancellationToken cancellationToken)
        {
            await _extensiveDomainService.PerformPassthroughAsync(new PassthroughObj(
                baseAttr: request.BaseAttr,
                concreteAttr: request.ConcreteAttr), cancellationToken);
        }
    }
}