using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.UpdateNonStringPartitionKey
{
    public class UpdateNonStringPartitionKeyCommand : IRequest, ICommand
    {
        public UpdateNonStringPartitionKeyCommand(string id, int partInt, string name)
        {
            Id = id;
            PartInt = partInt;
            Name = name;
        }

        public string Id { get; set; }
        public int PartInt { get; set; }
        public string Name { get; set; }
    }
}