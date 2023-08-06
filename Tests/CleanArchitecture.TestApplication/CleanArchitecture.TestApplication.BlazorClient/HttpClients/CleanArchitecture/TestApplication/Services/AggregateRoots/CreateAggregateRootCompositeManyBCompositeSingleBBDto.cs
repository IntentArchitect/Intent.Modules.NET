using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients.CleanArchitecture.TestApplication.Services.AggregateRoots
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