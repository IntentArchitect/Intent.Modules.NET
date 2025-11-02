using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace CleanArchitecture.QueueStorage.Eventing.Messages
{
    public record CreateProductCommand
    {
        public CreateProductCommand()
        {
            Name = null!;
        }

        public string Name { get; init; }
        public int Qty { get; init; }
    }
}