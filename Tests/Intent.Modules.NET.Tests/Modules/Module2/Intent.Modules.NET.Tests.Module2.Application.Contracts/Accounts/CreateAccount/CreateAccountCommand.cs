using Intent.Modules.NET.Tests.Application.Core.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.Contracts.Accounts.CreateAccount
{
    public class CreateAccountCommand : IRequest<Guid>, ICommand
    {
        public CreateAccountCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}