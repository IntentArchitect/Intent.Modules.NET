using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.DeleteNonStringPartitionKey
{
    public class DeleteNonStringPartitionKeyCommand : IRequest, ICommand
    {
        public DeleteNonStringPartitionKeyCommand(string id, int partInt)
        {
            Id = id;
            PartInt = partInt;
        }

        public string Id { get; set; }
        public int PartInt { get; set; }
    }
}