using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SwashbuckleSettings.All.Application.ReplaceWidget
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ReplaceWidgetHandler : IRequestHandler<ReplaceWidget>
    {
        [IntentManaged(Mode.Merge)]
        public ReplaceWidgetHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(ReplaceWidget request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (ReplaceWidgetHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}