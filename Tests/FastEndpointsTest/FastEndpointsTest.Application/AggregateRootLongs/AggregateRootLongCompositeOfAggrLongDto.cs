using AutoMapper;
using FastEndpointsTest.Application.Common.Mappings;
using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs
{
    public class AggregateRootLongCompositeOfAggrLongDto : IMapFrom<CompositeOfAggrLong>
    {
        public AggregateRootLongCompositeOfAggrLongDto()
        {
            Attribute = null!;
        }

        public string Attribute { get; set; }
        public long Id { get; set; }

        public static AggregateRootLongCompositeOfAggrLongDto Create(string attribute, long id)
        {
            return new AggregateRootLongCompositeOfAggrLongDto
            {
                Attribute = attribute,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeOfAggrLong, AggregateRootLongCompositeOfAggrLongDto>();
        }
    }
}