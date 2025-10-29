using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupA.Eventing.Messages
{
    public record CreateOrderCommand
    {
        public CreateOrderCommand()
        {
            Name = null!;
        }

        public string Name { get; init; }
    }
}