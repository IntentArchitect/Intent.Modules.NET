using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCommandCompositeDto
    {
        public CreateAggregateRootCompositeManyBCommandCompositeDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCompositeManyBCommandCompositeDto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeManyBCommandCompositeDto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}