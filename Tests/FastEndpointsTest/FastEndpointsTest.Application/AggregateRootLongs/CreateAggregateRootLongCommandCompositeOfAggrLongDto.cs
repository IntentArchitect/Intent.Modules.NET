using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs
{
    public record CreateAggregateRootLongCommandCompositeOfAggrLongDto
    {
        public CreateAggregateRootLongCommandCompositeOfAggrLongDto()
        {
            Attribute = null!;
        }

        public string Attribute { get; init; }

        public static CreateAggregateRootLongCommandCompositeOfAggrLongDto Create(string attribute)
        {
            return new CreateAggregateRootLongCommandCompositeOfAggrLongDto
            {
                Attribute = attribute
            };
        }
    }
}