using Intent.RoslynWeaver.Attributes;
using MediatR;
using Standard.AspNetCore.Serilog.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Standard.AspNetCore.Serilog.Application.Testing.PerformLargeObjectLogging
{
    public class PerformLargeObjectLogging : IRequest, ICommand
    {
        public PerformLargeObjectLogging()
        {
        }
    }
}