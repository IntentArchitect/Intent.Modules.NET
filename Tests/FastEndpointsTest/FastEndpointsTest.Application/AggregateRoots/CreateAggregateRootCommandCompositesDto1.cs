using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositesDto1
    {
        public CreateAggregateRootCommandCompositesDto1()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCommandCompositesDto1 Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositesDto1
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}