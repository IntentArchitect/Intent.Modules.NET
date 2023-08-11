using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Services.AggregateRoots
{
    public class UpdateAggregateRootCompositeSingleACompositeManyAADto
    {
        [Required(ErrorMessage = "Composite attr is required.")]
        public string CompositeAttr { get; set; }
        public Guid CompositeSingleAId { get; set; }
        public Guid Id { get; set; }

        public static UpdateAggregateRootCompositeSingleACompositeManyAADto Create(
            string compositeAttr,
            Guid compositeSingleAId,
            Guid id)
        {
            return new UpdateAggregateRootCompositeSingleACompositeManyAADto
            {
                CompositeAttr = compositeAttr,
                CompositeSingleAId = compositeSingleAId,
                Id = id
            };
        }
    }
}