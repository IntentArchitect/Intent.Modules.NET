using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class UpdateAggregateRootAggregateSingleCDto
    {
        public string AggregationAttr { get; set; }
        public Guid Id { get; set; }

        public static UpdateAggregateRootAggregateSingleCDto Create(string aggregationAttr, Guid id)
        {
            return new UpdateAggregateRootAggregateSingleCDto
            {
                AggregationAttr = aggregationAttr,
                Id = id
            };
        }
    }
}