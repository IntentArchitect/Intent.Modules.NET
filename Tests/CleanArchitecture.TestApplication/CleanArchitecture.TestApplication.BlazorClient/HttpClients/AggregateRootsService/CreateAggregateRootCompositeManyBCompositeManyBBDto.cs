using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class CreateAggregateRootCompositeManyBCompositeManyBBDto
    {
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