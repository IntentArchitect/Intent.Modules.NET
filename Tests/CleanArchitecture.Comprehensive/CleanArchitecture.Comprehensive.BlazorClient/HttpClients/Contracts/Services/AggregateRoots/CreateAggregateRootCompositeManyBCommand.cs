using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCommand
    {
        public CreateAggregateRootCompositeManyBCommand()
        {
            CompositeAttr = null!;
            Composites = [];
        }

        public Guid AggregateRootId { get; set; }
        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public CreateAggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }
        [Required(ErrorMessage = "Composites is required.")]
        public List<CreateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

        public static CreateAggregateRootCompositeManyBCommand Create(
            Guid aggregateRootId,
            string compositeAttr,
            DateTime? someDate,
            CreateAggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<CreateAggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            return new CreateAggregateRootCompositeManyBCommand
            {
                AggregateRootId = aggregateRootId,
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composite = composite,
                Composites = composites
            };
        }
    }
}