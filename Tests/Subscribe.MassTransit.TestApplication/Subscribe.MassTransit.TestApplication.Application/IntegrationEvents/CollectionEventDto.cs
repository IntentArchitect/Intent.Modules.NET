using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace Subscribe.MassTransit.TestApplication.Eventing.Messages
{
    public class CollectionEventDto
    {
        public CollectionEventDto()
        {
        }

        public string Field { get; set; }

        public static CollectionEventDto Create(string field)
        {
            return new CollectionEventDto
            {
                Field = field
            };
        }
    }
}