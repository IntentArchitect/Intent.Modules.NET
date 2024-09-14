using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBDto
    {
        public CreateAggregateRootCompositeManyBDto()
        {
            CompositeAttr = null!;
            Composites = [];
        }

        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public CreateAggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }
        [Required(ErrorMessage = "Composites is required.")]
        public List<CreateAggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

        public static CreateAggregateRootCompositeManyBDto Create(
            string compositeAttr,
            DateTime? someDate,
            CreateAggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<CreateAggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            return new CreateAggregateRootCompositeManyBDto
            {
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                Composite = composite,
                Composites = composites
            };
        }
    }
}