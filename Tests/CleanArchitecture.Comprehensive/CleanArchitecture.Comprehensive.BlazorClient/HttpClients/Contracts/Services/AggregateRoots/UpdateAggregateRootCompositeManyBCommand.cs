using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class UpdateAggregateRootCompositeManyBCommand
    {
        public UpdateAggregateRootCompositeManyBCommand()
        {
            CompositeAttr = null!;
            Composites = [];
        }

        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public UpdateAggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }
        [Required(ErrorMessage = "Composites is required.")]
        public List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

        public static UpdateAggregateRootCompositeManyBCommand Create(
            Guid aggregateRootId,
            Guid id,
            string compositeAttr,
            DateTime? someDate,
            UpdateAggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<UpdateAggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            return new UpdateAggregateRootCompositeManyBCommand
            {
                AggregateRootId = aggregateRootId,
                Id = id,
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composite = composite,
                Composites = composites
            };
        }
    }
}