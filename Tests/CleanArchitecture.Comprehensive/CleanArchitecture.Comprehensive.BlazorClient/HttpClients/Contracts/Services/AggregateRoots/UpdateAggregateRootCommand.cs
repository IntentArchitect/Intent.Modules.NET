using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class UpdateAggregateRootCommand
    {
        public UpdateAggregateRootCommand()
        {
            AggregateAttr = null!;
            Composites = [];
            LimitedDomain = null!;
            LimitedService = null!;
        }

        public Guid Id { get; set; }
        [Required(ErrorMessage = "Aggregate attr is required.")]
        public string AggregateAttr { get; set; }
        [Required(ErrorMessage = "Composites is required.")]
        public List<UpdateAggregateRootCompositeManyBDto> Composites { get; set; }
        public UpdateAggregateRootCompositeSingleADto? Composite { get; set; }
        public UpdateAggregateRootAggregateSingleCDto? Aggregate { get; set; }
        [Required(ErrorMessage = "Limited domain is required.")]
        [MaxLength(10, ErrorMessage = "Limited domain must be 10 or less characters.")]
        public string LimitedDomain { get; set; }
        [Required(ErrorMessage = "Limited service is required.")]
        [MaxLength(20, ErrorMessage = "Limited service must be 20 or less characters.")]
        public string LimitedService { get; set; }

        public static UpdateAggregateRootCommand Create(
            Guid id,
            string aggregateAttr,
            List<UpdateAggregateRootCompositeManyBDto> composites,
            UpdateAggregateRootCompositeSingleADto? composite,
            UpdateAggregateRootAggregateSingleCDto? aggregate,
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