using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Services.AggregateRoots
{
    public class AggregateRootCompositeManyBDto
    {
        public AggregateRootCompositeManyBDto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }
        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
        public AggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }
        [Required(ErrorMessage = "Composites is required.")]
        public List<AggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

        public static AggregateRootCompositeManyBDto Create(
            string compositeAttr,
            DateTime? someDate,
            Guid aggregateRootId,
            Guid id,
            AggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<AggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            return new AggregateRootCompositeManyBDto
            {
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                AggregateRootId = aggregateRootId,
                Id = id,
                Composite = composite,
                Composites = composites
            };
        }
    }
}