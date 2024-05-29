using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs
{
    public class CreateAggregateRootLongCompositeOfAggrLongDto
    {
        public CreateAggregateRootLongCompositeOfAggrLongDto()
        {
            Attribute = null!;
        }

        public string Attribute { get; set; }

        public static CreateAggregateRootLongCompositeOfAggrLongDto Create(string attribute)
        {
            return new CreateAggregateRootLongCompositeOfAggrLongDto
            {
                Attribute = attribute
            };
        }
    }
}