using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Interfaces;
using Redis.Om.Repositories.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Redis.Om.Repositories.Application.Clients.UpdateClientByOp
{
    public class UpdateClientByOpCommand : IRequest, ICommand
    {
        public UpdateClientByOpCommand(string id, ClientType type, string name)
        {
            Id = id;
            Type = type;
            Name = name;
        }

        public string Id { get; set; }
        public ClientType Type { get; set; }
        public string Name { get; set; }
    }
}