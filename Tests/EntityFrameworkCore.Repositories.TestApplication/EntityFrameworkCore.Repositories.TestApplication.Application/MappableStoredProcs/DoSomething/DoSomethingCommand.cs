using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.DoSomething
{
    public class DoSomethingCommand : IRequest, ICommand
    {
        public DoSomethingCommand()
        {
        }
    }
}