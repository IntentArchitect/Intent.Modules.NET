using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.CreateWidget
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateWidgetCommandHandler : IRequestHandler<CreateWidgetCommand, Guid>
    {
        [IntentManaged(Mode.Merge)]
        public CreateWidgetCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Guid> Handle(CreateWidgetCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            InMemoryWidgetStore.Widgets[id] = new WidgetDto { Id = id, Name = request.Name };
            return id;
        }
    }
}
