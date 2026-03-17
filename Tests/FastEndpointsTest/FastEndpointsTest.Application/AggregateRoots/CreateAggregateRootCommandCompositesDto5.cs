using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record CreateAggregateRootCommandCompositesDto5
    {
        public CreateAggregateRootCommandCompositesDto5()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; init; }

        public static CreateAggregateRootCommandCompositesDto5 Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositesDto5
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}