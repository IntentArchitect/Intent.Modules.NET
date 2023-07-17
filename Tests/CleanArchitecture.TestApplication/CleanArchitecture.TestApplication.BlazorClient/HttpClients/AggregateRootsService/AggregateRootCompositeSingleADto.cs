using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class AggregateRootCompositeSingleADto
    {
        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }
        public AggregateRootCompositeSingleACompositeSingleAADto Composite { get; set; }
        public List<AggregateRootCompositeSingleACompositeManyAADto> Composites { get; set; }

        public static AggregateRootCompositeSingleADto Create(
            string compositeAttr,
            Guid id,
            AggregateRootCompositeSingleACompositeSingleAADto composite,
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