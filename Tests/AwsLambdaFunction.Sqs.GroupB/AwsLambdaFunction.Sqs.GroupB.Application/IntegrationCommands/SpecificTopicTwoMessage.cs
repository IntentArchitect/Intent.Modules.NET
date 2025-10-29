using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace AwsLambdaFunction.Sqs.GroupB.Eventing.Messages
{
    public record SpecificTopicTwoMessage
    {
        public SpecificTopicTwoMessage()
        {
            FieldB = null!;
        }

        public string FieldB { get; init; }
    }
}