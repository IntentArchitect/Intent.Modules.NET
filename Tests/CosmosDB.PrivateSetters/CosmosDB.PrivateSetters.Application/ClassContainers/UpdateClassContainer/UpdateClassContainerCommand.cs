using CosmosDB.PrivateSetters.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.PrivateSetters.Application.ClassContainers.UpdateClassContainer
{
    public class UpdateClassContainerCommand : IRequest, ICommand
    {
        public UpdateClassContainerCommand(string id, string classPartitionKey)
        {
            Id = id;
            ClassPartitionKey = classPartitionKey;
        }

        public string Id { get; set; }
        public string ClassPartitionKey { get; set; }
    }
}