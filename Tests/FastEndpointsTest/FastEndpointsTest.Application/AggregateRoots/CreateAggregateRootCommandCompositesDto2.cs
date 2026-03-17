using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record CreateAggregateRootCommandCompositesDto2
    {
        public CreateAggregateRootCommandCompositesDto2()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; init; }

        public static CreateAggregateRootCommandCompositesDto2 Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositesDto2
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}