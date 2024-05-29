using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class CreateAggregateRootAggregateSingleCDto
    {
        public CreateAggregateRootAggregateSingleCDto()
        {
            AggregationAttr = null!;
        }

        [Required(ErrorMessage = "Aggregation attr is required.")]
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