using CosmosDB.EntityInterfaces.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Application.Clients.DeleteClient
{
    public class DeleteClientCommand : IRequest, ICommand
    {
        public DeleteClientCommand(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; set; }
    }
}