using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class CreateAggregateRootCompositeSingleACompositeManyAADto
    {
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