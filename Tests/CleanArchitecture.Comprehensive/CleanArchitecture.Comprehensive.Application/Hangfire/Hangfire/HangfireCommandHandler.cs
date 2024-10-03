using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Hangfire.Hangfire
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class HangfireCommandHandler : IRequestHandler<HangfireCommand>
    {
        [IntentManaged(Mode.Merge)]
        public HangfireCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(HangfireCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (HangfireCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}