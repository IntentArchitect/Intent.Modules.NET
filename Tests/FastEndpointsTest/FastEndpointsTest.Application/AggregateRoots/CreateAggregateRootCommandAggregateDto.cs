using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandAggregateDto
    {
        public CreateAggregateRootCommandAggregateDto()
        {
            AggregationAttr = null!;
        }

        public string AggregationAttr { get; set; }

        public static CreateAggregateRootCommandAggregateDto Create(string aggregationAttr)
        {
            return new CreateAggregateRootCommandAggregateDto
            {
                AggregationAttr = aggregationAttr
            };
        }
    }
}