using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class UpdateAggregateRootCompositeManyBCompositeSingleBBDto
    {
        public UpdateAggregateRootCompositeManyBCompositeSingleBBDto()
        {
            CompositeAttr = null!;
        }

        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }

        public static UpdateAggregateRootCompositeManyBCompositeSingleBBDto Create(string compositeAttr, Guid id)
        {
            return new UpdateAggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = compositeAttr,
                Id = id
            };
        }
    }
}