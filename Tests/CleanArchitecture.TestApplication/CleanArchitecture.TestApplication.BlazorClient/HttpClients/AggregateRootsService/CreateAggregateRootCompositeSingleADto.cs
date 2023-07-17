using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class CreateAggregateRootCompositeSingleADto
    {
        public string CompositeAttr { get; set; }
        public CreateAggregateRootCompositeSingleACompositeSingleAADto Composite { get; set; }
        public List<CreateAggregateRootCompositeSingleACompositeManyAADto> Composites { get; set; }

        public static CreateAggregateRootCompositeSingleADto Create(
            string compositeAttr,
            CreateAggregateRootCompositeSingleACompositeSingleAADto composite,
            List<CreateAggregateRootCompositeSingleACompositeManyAADto> composites)
        {
            return new CreateAggregateRootCompositeSingleADto
            {
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites
            };
        }
    }
}