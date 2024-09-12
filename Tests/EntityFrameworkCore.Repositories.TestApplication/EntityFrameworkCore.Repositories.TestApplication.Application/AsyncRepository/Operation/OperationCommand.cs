using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.AsyncRepository.Operation
{
    public class OperationCommand : IRequest, ICommand
    {
        public OperationCommand()
        {
        }
    }
}