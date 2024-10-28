using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots
{
    public class CreateAggregateRootCommandCompositesDto5
    {
        public CreateAggregateRootCommandCompositesDto5()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCommandCompositesDto5 Create(string compositeAttr)
        {
            return new CreateAggregateRootCommandCompositesDto5
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}