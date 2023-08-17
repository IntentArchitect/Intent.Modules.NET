using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Services.AggregateRoots
{
    public class AggregateRootCompositeManyBCompositeSingleBBDto
    {
        public AggregateRootCompositeManyBCompositeSingleBBDto()
        {
            CompositeAttr = null!;
        }
        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }

        public static AggregateRootCompositeManyBCompositeSingleBBDto Create(string compositeAttr, Guid id)
        {
            return new AggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = compositeAttr,
                Id = id
            };
        }
    }
}