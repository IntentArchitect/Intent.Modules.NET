using Hangfire.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Hangfire.Tests.Application.Job
{
    public class JobCommand : IRequest, ICommand
    {
        public JobCommand()
        {
        }
    }
}