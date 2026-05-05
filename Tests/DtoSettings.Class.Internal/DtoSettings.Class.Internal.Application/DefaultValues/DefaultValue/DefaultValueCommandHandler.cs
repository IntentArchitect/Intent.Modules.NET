using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace DtoSettings.Class.Internal.Application.DefaultValues.DefaultValue
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DefaultValueCommandHandler : IRequestHandler<DefaultValueCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DefaultValueCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(DefaultValueCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (DefaultValueCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}