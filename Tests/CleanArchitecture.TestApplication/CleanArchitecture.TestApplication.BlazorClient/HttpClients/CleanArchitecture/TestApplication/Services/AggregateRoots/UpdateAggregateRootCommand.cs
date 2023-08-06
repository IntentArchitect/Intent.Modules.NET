using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.CleanArchitecture.TestApplication.Services.AggregateRoots
{
    public class UpdateAggregateRootCommand
    {
        public Guid Id { get; set; }
        public string AggregateAttr { get; set; }
        public List<UpdateAggregateRootCompositeManyBDto> Composites { get; set; }
        public UpdateAggregateRootCompositeSingleADto Composite { get; set; }
        public UpdateAggregateRootAggregateSingleCDto Aggregate { get; set; }
        public string LimitedDomain { get; set; }
        public string LimitedService { get; set; }

        public static UpdateAggregateRootCommand Create(
            Guid id,
            string aggregateAttr,
            List<UpdateAggregateRootCompositeManyBDto> composites,
            UpdateAggregateRootCompositeSingleADto composite,
            UpdateAggregateRootAggregateSingleCDto aggregate,
            string limitedDomain,
            string limitedService)
        {
            return new UpdateAggregateRootCommand
            {
                Id = id,
                AggregateAttr = aggregateAttr,
                Composites = composites,
                Composite = composite,
                Aggregate = aggregate,
                LimitedDomain = limitedDomain,
                LimitedService = limitedService
            };
        }
    }
}