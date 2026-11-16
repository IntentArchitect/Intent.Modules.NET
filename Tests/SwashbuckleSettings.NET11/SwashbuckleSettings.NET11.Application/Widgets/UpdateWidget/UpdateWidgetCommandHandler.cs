using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.UpdateWidget
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateWidgetCommandHandler : IRequestHandler<UpdateWidgetCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateWidgetCommandHandler()
        {
        }

        /// <summary>
        /// The route carries {id} and the command body also has Id; the OpenAPI body schema must not show id.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(UpdateWidgetCommand request, CancellationToken cancellationToken)
        {
            InMemoryWidgetStore.Widgets[request.Id] = new WidgetDto { Id = request.Id, Name = request.Name };
        }
    }
}
