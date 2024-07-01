using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Solace.Tests.Application.NotMapped.NotMappedTest
{
    public class NotMappedTestCommand : IRequest, ICommand
    {
        public NotMappedTestCommand()
        {
        }
    }
}