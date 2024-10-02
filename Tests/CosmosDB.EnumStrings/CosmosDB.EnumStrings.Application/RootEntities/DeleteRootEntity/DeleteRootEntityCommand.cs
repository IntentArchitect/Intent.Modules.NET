using CosmosDB.EnumStrings.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.DeleteRootEntity
{
    public class DeleteRootEntityCommand : IRequest, ICommand
    {
        public DeleteRootEntityCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}