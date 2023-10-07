using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.ClassContainers.UpdateClassContainer
{
    public class UpdateClassContainerCommand : IRequest, ICommand
    {
        public UpdateClassContainerCommand(string id, string classPartitionKey)
        {
            Id = id;
            ClassPartitionKey = classPartitionKey;
        }

        public string Id { get; private set; }
        public string ClassPartitionKey { get; set; }

        public void SetId(string id)
        {
            if (Id == default)
            {
                Id = id;
            }
        }
    }
}