using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace Scalar.NET9.Bearer.Application.Interfaces
{
    public interface IWidgetService
    {
        Task CreateWidget(WidgetDto widget, CancellationToken cancellationToken = default);
        Task UpdateWidget(Guid widgetId, WidgetDto widget, CancellationToken cancellationToken = default);
    }
}