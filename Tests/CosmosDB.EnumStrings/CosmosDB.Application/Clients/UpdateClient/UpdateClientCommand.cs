using CosmosDB.Application.Common.Interfaces;
using CosmosDB.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.Clients.UpdateClient
{
    public class UpdateClientCommand : IRequest, ICommand
    {
        public UpdateClientCommand(string identifier, ClientType type, string name)
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