using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class AggregateRootAggregateSingleCDto
    {
        public AggregateRootAggregateSingleCDto()
        {
            AggregationAttr = null!;
        }

        public string AggregationAttr { get; set; }
        public Guid Id { get; set; }

        public static AggregateRootAggregateSingleCDto Create(string aggregationAttr, Guid id)
        {
            return new AggregateRootAggregateSingleCDto
            {
                AggregationAttr = aggregationAttr,
                Id = id
            };
        }
    }
}