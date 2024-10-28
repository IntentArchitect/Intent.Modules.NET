using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositesDto4
    {
        public CreateAggregateRootCommandCompositesDto4()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCommandCompositesDto4 Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositesDto4
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}