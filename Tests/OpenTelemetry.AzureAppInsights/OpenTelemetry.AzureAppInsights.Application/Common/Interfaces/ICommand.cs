using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandInterface", Version = "1.0")]

namespace OpenTelemetry.AzureAppInsights.Application.Common.Interfaces
{
    public interface ICommand
    {

    }
}