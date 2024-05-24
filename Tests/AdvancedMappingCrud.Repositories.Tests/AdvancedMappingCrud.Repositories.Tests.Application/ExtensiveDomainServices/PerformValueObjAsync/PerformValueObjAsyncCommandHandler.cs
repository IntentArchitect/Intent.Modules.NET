using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.ExtensiveDomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.ExtensiveDomainServices;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.ExtensiveDomainServices.PerformValueObjAsync
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PerformValueObjAsyncCommandHandler : IRequestHandler<PerformValueObjAsyncCommand>
    {
        private readonly IExtensiveDomainService _extensiveDomainService;

        [IntentManaged(Mode.Merge)]
        public PerformValueObjAsyncCommandHandler(IExtensiveDomainService extensiveDomainService)
        {
            _extensiveDomainService = extensiveDomainService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(PerformValueObjAsyncCommand request, CancellationToken cancellationToken)
        {
            await _extensiveDomainService.PerformValueObjAsync(new SimpleVO(
                value1: request.Value1,
                value2: request.Value2), cancellationToken);
        }
    }
}