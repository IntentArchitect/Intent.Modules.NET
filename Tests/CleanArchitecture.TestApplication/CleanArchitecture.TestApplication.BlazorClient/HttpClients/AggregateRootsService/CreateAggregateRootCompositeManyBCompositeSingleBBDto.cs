using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class CreateAggregateRootCompositeManyBCompositeSingleBBDto
    {
        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCompositeManyBCompositeSingleBBDto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}