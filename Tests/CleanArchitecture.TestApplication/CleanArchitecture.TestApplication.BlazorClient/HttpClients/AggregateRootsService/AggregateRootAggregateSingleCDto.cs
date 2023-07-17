using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class AggregateRootAggregateSingleCDto
    {
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