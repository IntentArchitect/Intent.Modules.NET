using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs
{
    public class UpdateAggregateRootLongCompositeOfAggrLongDto
    {
        public UpdateAggregateRootLongCompositeOfAggrLongDto()
        {
            Attribute = null!;
        }

        public string Attribute { get; set; }
        public long Id { get; set; }

        public static UpdateAggregateRootLongCompositeOfAggrLongDto Create(string attribute, long id)
        {
            return new UpdateAggregateRootLongCompositeOfAggrLongDto
            {
                Attribute = attribute,
                Id = id
            };
        }
    }
}