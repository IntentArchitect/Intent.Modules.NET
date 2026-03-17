using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record CreateAggregateRootCommandCompositesDto1
    {
        public CreateAggregateRootCommandCompositesDto1()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; init; }

        public static CreateAggregateRootCommandCompositesDto1 Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositesDto1
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}