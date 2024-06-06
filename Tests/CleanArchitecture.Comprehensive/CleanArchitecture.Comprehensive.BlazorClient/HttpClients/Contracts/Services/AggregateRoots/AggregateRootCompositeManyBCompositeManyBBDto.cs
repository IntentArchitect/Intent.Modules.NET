using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class AggregateRootCompositeManyBCompositeManyBBDto
    {
        public AggregateRootCompositeManyBCompositeManyBBDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid CompositeManyBId { get; set; }
        public Guid Id { get; set; }

        public static AggregateRootCompositeManyBCompositeManyBBDto Create(
            string compositeAttr,
            Guid compositeManyBId,
            Guid id)
        {
            return new AggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = compositeAttr,
                CompositeManyBId = compositeManyBId,
                Id = id
            };
        }
    }
}