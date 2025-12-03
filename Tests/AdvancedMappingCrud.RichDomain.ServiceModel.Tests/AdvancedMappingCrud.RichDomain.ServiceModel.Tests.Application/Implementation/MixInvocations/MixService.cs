using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces.MixInvocations;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.MixInvocations.CreateItem;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Implementation.MixInvocations
{
    [IntentManaged(Mode.Merge)]
    public class MixService : IMixService
    {
        private readonly ISender _mediator;

        [IntentManaged(Mode.Merge)]
        public MixService(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateItem(string name, CancellationToken cancellationToken = default)
        {
            var command = new CreateItemCommand(
                name: name);
            var result = await _mediator.Send(command, cancellationToken);
            return result;
        }
    }
}