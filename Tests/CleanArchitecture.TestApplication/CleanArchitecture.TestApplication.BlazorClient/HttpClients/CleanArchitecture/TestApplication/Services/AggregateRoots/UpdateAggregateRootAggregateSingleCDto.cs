using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.CleanArchitecture.TestApplication.Services.AggregateRoots
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