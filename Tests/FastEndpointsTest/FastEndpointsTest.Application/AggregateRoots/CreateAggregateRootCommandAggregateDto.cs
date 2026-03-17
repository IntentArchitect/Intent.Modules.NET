using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record CreateAggregateRootCommandAggregateDto
    {
        public CreateAggregateRootCommandAggregateDto()
        {
            AggregationAttr = null!;
        }

        public string AggregationAttr { get; init; }

        public static CreateAggregateRootCommandAggregateDto Create(string aggregationAttr)
        {
            return new CreateAggregateRootCommandAggregateDto
            {
                AggregationAttr = aggregationAttr
            };
        }
    }
}