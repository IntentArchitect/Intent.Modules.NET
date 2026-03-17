using AutoMapper;
using FastEndpointsTest.Application.Common.Mappings;
using FastEndpointsTest.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRootLongs
{
    public record AggregateRootLongDto : IMapFrom<AggregateRootLong>
    {
        public AggregateRootLongDto()
        {
            Attribute = null!;
        }

        public long Id { get; init; }
        public string Attribute { get; init; }
        public AggregateRootLongCompositeOfAggrLongDto? CompositeOfAggrLong { get; init; }

        public static AggregateRootLongDto Create(
            long id,
            string attribute,
            AggregateRootLongCompositeOfAggrLongDto? compositeOfAggrLong)
        {
            return new AggregateRootLongDto
            {
                Id = id,
                Attribute = attribute,
                CompositeOfAggrLong = compositeOfAggrLong
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRootLong, AggregateRootLongDto>();
        }
    }
}