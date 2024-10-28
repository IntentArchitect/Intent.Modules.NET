using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositeDto2
    {
        public CreateAggregateRootCommandCompositeDto2()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCommandCompositeDto2 Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositeDto2
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}