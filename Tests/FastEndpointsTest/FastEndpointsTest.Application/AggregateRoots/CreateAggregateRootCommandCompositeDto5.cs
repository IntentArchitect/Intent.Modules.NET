using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositeDto5
    {
        public CreateAggregateRootCommandCompositeDto5()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCommandCompositeDto5 Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositeDto5
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}