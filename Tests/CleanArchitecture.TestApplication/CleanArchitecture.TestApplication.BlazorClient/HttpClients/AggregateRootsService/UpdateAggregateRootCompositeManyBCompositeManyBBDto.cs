using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class UpdateAggregateRootCompositeManyBCompositeManyBBDto
    {
        public string CompositeAttr { get; set; }
        public Guid CompositeManyBId { get; set; }
        public Guid Id { get; set; }

        public static UpdateAggregateRootCompositeManyBCompositeManyBBDto Create(
            string compositeAttr,
            Guid compositeManyBId,
            Guid id)
        {
            return new UpdateAggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = compositeAttr,
                CompositeManyBId = compositeManyBId,
                Id = id
            };
        }
    }
}