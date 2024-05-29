using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class CreateAggregateRootAggregateSingleCDto
    {
        public CreateAggregateRootAggregateSingleCDto()
        {
            AggregationAttr = null!;
        }

        public string AggregationAttr { get; set; }

        public static CreateAggregateRootAggregateSingleCDto Create(string aggregationAttr)
        {
            return new CreateAggregateRootAggregateSingleCDto
            {
                AggregationAttr = aggregationAttr
            };
        }
    }
}