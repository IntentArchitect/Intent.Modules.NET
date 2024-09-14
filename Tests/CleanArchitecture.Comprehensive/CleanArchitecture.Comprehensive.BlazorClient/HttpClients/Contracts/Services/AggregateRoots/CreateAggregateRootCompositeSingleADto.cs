using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class CreateAggregateRootCompositeSingleADto
    {
        public CreateAggregateRootCompositeSingleADto()
        {
            CompositeAttr = null!;
            Composites = [];
        }

        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public CreateAggregateRootCompositeSingleACompositeSingleAADto? Composite { get; set; }
        [Required(ErrorMessage = "Composites is required.")]
        public List<CreateAggregateRootCompositeSingleACompositeManyAADto> Composites { get; set; }

        public static CreateAggregateRootCompositeSingleADto Create(
            string compositeAttr,
            CreateAggregateRootCompositeSingleACompositeSingleAADto? composite,
            List<CreateAggregateRootCompositeSingleACompositeManyAADto> composites)
        {
            return new CreateAggregateRootCompositeSingleADto
            {
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites
            };
        }
    }
}