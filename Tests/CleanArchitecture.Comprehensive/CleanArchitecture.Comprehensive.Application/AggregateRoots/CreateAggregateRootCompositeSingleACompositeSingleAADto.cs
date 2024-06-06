using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class CreateAggregateRootCompositeSingleACompositeSingleAADto
    {
        public CreateAggregateRootCompositeSingleACompositeSingleAADto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }

        public static CreateAggregateRootCompositeSingleACompositeSingleAADto Create(string compositeAttr)
        {
            return new CreateAggregateRootCompositeSingleACompositeSingleAADto
            {
                CompositeAttr = compositeAttr
            };
        }
    }
}