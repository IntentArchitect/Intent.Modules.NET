using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace SwashbuckleSettings.NET11.Application.Widgets.PatchWidget
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PatchWidgetCommandHandler : IRequestHandler<PatchWidgetCommand, WidgetDto>
    {
        [IntentManaged(Mode.Merge)]
        public PatchWidgetCommandHandler()
        {
        }

        /// <summary>
        /// JSON merge patch (RFC 7396) of a widget's name.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<WidgetDto> Handle(PatchWidgetCommand request, CancellationToken cancellationToken)
        {
            if (!InMemoryWidgetStore.Widgets.TryGetValue(request.Id, out var existing))
            {
                return null!;
            }

            request.Name = existing.Name;
            await request.PatchExecutor.ApplyToAsync(request, cancellationToken);

            var patched = new WidgetDto { Id = request.Id, Name = request.Name ?? existing.Name };
            InMemoryWidgetStore.Widgets[request.Id] = patched;
            return patched;
        }
    }
}
