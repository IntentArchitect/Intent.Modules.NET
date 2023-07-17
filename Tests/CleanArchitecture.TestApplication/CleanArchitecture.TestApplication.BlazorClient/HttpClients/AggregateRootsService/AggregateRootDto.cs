using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class AggregateRootDto
    {
        public Guid Id { get; set; }
        public string AggregateAttr { get; set; }
        public List<AggregateRootCompositeManyBDto> Composites { get; set; }
        public AggregateRootCompositeSingleADto Composite { get; set; }
        public AggregateRootAggregateSingleCDto Aggregate { get; set; }

        public static AggregateRootDto Create(
            Guid id,
            string aggregateAttr,
            List<AggregateRootCompositeManyBDto> composites,
            AggregateRootCompositeSingleADto composite,
            AggregateRootAggregateSingleCDto aggregate)
        {
            return new AggregateRootDto
            {
                Id = id,
                AggregateAttr = aggregateAttr,
                Composites = composites,
                Composite = composite,
                Aggregate = aggregate
            };
        }
    }
}