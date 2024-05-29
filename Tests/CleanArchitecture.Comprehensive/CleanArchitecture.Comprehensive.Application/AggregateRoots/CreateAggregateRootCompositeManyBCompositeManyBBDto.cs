using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCompositeManyBBDto
    {
        public CreateAggregateRootCompositeManyBCompositeManyBBDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCompositeManyBCompositeManyBBDto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}