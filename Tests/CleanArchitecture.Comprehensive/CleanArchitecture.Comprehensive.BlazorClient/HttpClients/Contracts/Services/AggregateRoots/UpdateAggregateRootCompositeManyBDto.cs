using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class UpdateAggregateRootCompositeManyBDto
    {
        public UpdateAggregateRootCompositeManyBDto()
        {
            CompositeAttr = null!;
            Composites = [];
        }

        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Composites is required.")]
        public List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }
        public UpdateAggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }

        public static UpdateAggregateRootCompositeManyBDto Create(
            string compositeAttr,
            DateTime? someDate,
            Guid aggregateRootId,
            Guid id,
            List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> composites,
            UpdateAggregateRootCompositeManyBCompositeSingleBBDto? composite)
        {
            return new UpdateAggregateRootCompositeManyBDto
            {
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                AggregateRootId = aggregateRootId,
                Id = id,
                Composites = composites,
                Composite = composite
            };
        }
    }
}