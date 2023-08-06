using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.CleanArchitecture.TestApplication.Services.AggregateRoots
{
    public class CreateAggregateRootCommand
    {
        public string AggregateAttr { get; set; }
        public List<CreateAggregateRootCompositeManyBDto> Composites { get; set; }
        public CreateAggregateRootCompositeSingleADto Composite { get; set; }
        public CreateAggregateRootAggregateSingleCDto Aggregate { get; set; }
        public string LimitedDomain { get; set; }
        public string LimitedService { get; set; }

        public static CreateAggregateRootCommand Create(
            string aggregateAttr,
            List<CreateAggregateRootCompositeManyBDto> composites,
            CreateAggregateRootCompositeSingleADto composite,
            CreateAggregateRootAggregateSingleCDto aggregate,
            string limitedDomain,
            string limitedService)
        {
            return new CreateAggregateRootCommand
            {
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