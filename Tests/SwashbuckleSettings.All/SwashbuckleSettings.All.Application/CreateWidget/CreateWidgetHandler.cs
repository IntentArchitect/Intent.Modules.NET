using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SwashbuckleSettings.All.Application.CreateWidget
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateWidgetHandler : IRequestHandler<CreateWidget>
    {
        [IntentManaged(Mode.Merge)]
        public CreateWidgetHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(CreateWidget request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CreateWidgetHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}