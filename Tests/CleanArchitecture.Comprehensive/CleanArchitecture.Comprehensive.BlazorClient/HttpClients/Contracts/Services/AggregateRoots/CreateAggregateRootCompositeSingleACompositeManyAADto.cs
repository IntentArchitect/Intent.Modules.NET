using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class CreateAggregateRootCompositeSingleACompositeManyAADto
    {
        public CreateAggregateRootCompositeSingleACompositeManyAADto()
        {
            CompositeAttr = null!;
        }

        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCompositeSingleACompositeManyAADto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeSingleACompositeManyAADto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}