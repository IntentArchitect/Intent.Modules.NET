using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "1.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.AggregateRootsService
{
    public class CreateAggregateRootCompositeSingleACompositeSingleAADto
    {
        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCompositeSingleACompositeSingleAADto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeSingleACompositeSingleAADto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}