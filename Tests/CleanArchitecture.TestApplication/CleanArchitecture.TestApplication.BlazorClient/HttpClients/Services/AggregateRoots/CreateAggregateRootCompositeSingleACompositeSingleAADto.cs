using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.Services.AggregateRoots
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