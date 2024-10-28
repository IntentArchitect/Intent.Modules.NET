using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositesDto2
    {
        public CreateAggregateRootCommandCompositesDto2()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCommandCompositesDto2 Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositesDto2
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}