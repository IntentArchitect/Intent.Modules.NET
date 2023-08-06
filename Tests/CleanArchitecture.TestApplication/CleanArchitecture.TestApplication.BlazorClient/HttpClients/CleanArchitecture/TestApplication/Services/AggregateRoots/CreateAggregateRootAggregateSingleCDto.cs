using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.CleanArchitecture.TestApplication.Services.AggregateRoots
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