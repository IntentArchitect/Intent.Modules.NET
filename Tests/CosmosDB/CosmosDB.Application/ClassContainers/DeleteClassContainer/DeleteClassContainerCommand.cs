using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.ClassContainers.DeleteClassContainer
{
    public class DeleteClassContainerCommand : IRequest, ICommand
    {
        public DeleteClassContainerCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}