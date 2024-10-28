using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs
{
    public class UpdateAggregateRootLongCommandCompositeOfAggrLongDto
    {
        public UpdateAggregateRootLongCommandCompositeOfAggrLongDto()
        {
            Attribute = null!;
        }

        public string Attribute { get; set; }

        public static UpdateAggregateRootLongCommandCompositeOfAggrLongDto Create(string attribute)
        {
            return new UpdateAggregateRootLongCommandCompositeOfAggrLongDto
            {
                Attribute = attribute
            };
        }
    }
}