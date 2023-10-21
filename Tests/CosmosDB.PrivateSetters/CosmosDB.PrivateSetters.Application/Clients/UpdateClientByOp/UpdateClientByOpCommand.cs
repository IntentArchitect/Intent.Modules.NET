using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using CosmosDB.PrivateSetters.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.Clients.UpdateClientByOp
{
    public class UpdateClientByOpCommand : IRequest, ICommand
    {
        public UpdateClientByOpCommand(string identifier, ClientType type, string name)
        {
            Identifier = identifier;
            Type = type;
            Name = name;
        }

        public string Identifier { get; set; }
        public ClientType Type { get; set; }
        public string Name { get; set; }
    }
}