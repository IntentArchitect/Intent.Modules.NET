using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.ClassContainers.CreateClassContainer
{
    public class CreateClassContainerCommand : IRequest<string>, ICommand
    {
        public CreateClassContainerCommand(string classPartitionKey)
        {
            ClassPartitionKey = classPartitionKey;
        }

        public string ClassPartitionKey { get; set; }
    }
}