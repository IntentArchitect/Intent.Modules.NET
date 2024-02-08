using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Interfaces;
using Redis.Om.Repositories.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Redis.Om.Repositories.Application.Clients.CreateClient
{
    public class CreateClientCommand : IRequest<string>, ICommand
    {
        public CreateClientCommand(ClientType type, string name)
        {
            Type = type;
            Name = name;
        }

        public ClientType Type { get; set; }
        public string Name { get; set; }
    }
}