using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public record CreateAggregateRootCompositeManyBCommandCompositesDto
    {
        public CreateAggregateRootCompositeManyBCommandCompositesDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; init; }

        public static CreateAggregateRootCompositeManyBCommandCompositesDto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeManyBCommandCompositesDto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}