using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenTelemetry.OpenTelemetryProtocol.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OpenTelemetry.OpenTelemetryProtocol.Application.Test
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