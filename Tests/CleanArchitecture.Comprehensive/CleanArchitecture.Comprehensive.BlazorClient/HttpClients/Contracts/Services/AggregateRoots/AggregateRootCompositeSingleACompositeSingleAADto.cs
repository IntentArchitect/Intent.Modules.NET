using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.AggregateRoots
{
    public class AggregateRootCompositeSingleACompositeSingleAADto
    {
        public AggregateRootCompositeSingleACompositeSingleAADto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }

        public static AggregateRootCompositeSingleACompositeSingleAADto Create(string compositeAttr, Guid id)
        {
            return new AggregateRootCompositeSingleACompositeSingleAADto
            {
                CompositeAttr = compositeAttr,
                Id = id
            };
        }
    }
}