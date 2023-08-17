using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Services.AggregateRoots
{
    public class AggregateRootCompositeSingleADto
    {
        public AggregateRootCompositeSingleADto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }
        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }
        public AggregateRootCompositeSingleACompositeSingleAADto? Composite { get; set; }
        [Required(ErrorMessage = "Composites is required.")]
        public List<AggregateRootCompositeSingleACompositeManyAADto> Composites { get; set; }

        public static AggregateRootCompositeSingleADto Create(
            string compositeAttr,
            Guid id,
            AggregateRootCompositeSingleACompositeSingleAADto? composite,
            List<AggregateRootCompositeSingleACompositeManyAADto> composites)
        {
            return new AggregateRootCompositeSingleADto
            {
                CompositeAttr = compositeAttr,
                Id = id,
                Composite = composite,
                Composites = composites
            };
        }
    }
}