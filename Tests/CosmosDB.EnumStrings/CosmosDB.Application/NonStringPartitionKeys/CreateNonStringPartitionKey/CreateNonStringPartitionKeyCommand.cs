using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.Application.NonStringPartitionKeys.CreateNonStringPartitionKey
{
    public class CreateNonStringPartitionKeyCommand : IRequest, ICommand
    {
        public CreateNonStringPartitionKeyCommand(int partInt, string name)
        {
            PartInt = partInt;
            Name = name;
        }

        public int PartInt { get; set; }
        public string Name { get; set; }
    }
}