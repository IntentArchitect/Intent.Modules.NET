using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositeDto
    {
        public CreateAggregateRootCommandCompositeDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCommandCompositeDto Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositeDto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}