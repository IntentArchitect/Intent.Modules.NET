using Intent.RoslynWeaver.Attributes;
using Scalar.NET10.Bearer.Application.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Scalar.NET10.Bearer.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class WidgetService : IWidgetService
    {
        [IntentManaged(Mode.Merge)]
        public WidgetService()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task CreateWidget(WidgetDto widget, CancellationToken cancellationToken = default)
        {
            // TODO: Implement CreateWidget (WidgetService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task UpdateWidget(Guid widgetId, WidgetDto widget, CancellationToken cancellationToken = default)
        {
            // TODO: Implement UpdateWidget (WidgetService) functionality
            throw new NotImplementedException("Write your implementation for this service here...");
        }

        /// <summary>
        /// HTTP QUERY endpoint: the search criteria travel in the request body.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<WidgetDto>> SearchWidgets(WidgetDto criteria, CancellationToken cancellationToken = default)
        {
            // This application has no persistence: the operation exists to prove the generated HTTP QUERY surface,
            // so there is never anything to match.
            await Task.CompletedTask;
            return [];
        }
    }
}
