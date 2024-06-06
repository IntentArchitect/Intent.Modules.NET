using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs
{
    public class AggregateRootLongDto : IMapFrom<AggregateRootLong>
    {
        public AggregateRootLongDto()
        {
            Attribute = null!;
        }

        public long Id { get; set; }
        public string Attribute { get; set; }
        public AggregateRootLongCompositeOfAggrLongDto? CompositeOfAggrLong { get; set; }

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