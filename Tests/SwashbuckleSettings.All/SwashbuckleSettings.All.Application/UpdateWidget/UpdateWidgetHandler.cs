using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SwashbuckleSettings.All.Application.UpdateWidget
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateWidgetHandler : IRequestHandler<UpdateWidget>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateWidgetHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(UpdateWidget request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (UpdateWidgetHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}