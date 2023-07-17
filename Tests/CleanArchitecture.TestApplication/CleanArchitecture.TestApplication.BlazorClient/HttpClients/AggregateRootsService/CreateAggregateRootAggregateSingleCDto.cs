using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class CreateAggregateRootAggregateSingleCDto
    {
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