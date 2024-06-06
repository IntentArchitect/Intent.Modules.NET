using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCompositeManyBBDto
    {
        public CreateAggregateRootCompositeManyBCompositeManyBBDto()
        {
            CompositeAttr = null!;
        }

        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCompositeManyBCompositeManyBBDto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}