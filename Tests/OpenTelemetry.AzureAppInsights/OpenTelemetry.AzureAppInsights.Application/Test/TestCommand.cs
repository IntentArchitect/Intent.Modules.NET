using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenTelemetry.AzureAppInsights.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OpenTelemetry.AzureAppInsights.Application.Test
{
    public class TestCommand : IRequest, ICommand
    {
        public TestCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}