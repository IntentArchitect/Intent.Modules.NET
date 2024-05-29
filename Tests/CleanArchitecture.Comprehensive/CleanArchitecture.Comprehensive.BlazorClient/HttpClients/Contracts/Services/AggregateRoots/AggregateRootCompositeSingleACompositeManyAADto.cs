using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class AggregateRootCompositeSingleACompositeManyAADto
    {
        public AggregateRootCompositeSingleACompositeManyAADto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid CompositeSingleAId { get; set; }
        public Guid Id { get; set; }

        public static AggregateRootCompositeSingleACompositeManyAADto Create(
            string compositeAttr,
            Guid compositeSingleAId,
            Guid id)
        {
            return new AggregateRootCompositeSingleACompositeManyAADto
            {
                CompositeAttr = compositeAttr,
                CompositeSingleAId = compositeSingleAId,
                Id = id
            };
        }
    }
}