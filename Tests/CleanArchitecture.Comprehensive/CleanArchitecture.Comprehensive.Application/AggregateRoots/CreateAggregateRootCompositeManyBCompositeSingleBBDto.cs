using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCompositeSingleBBDto
    {
        public CreateAggregateRootCompositeManyBCompositeSingleBBDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCompositeManyBCompositeSingleBBDto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}