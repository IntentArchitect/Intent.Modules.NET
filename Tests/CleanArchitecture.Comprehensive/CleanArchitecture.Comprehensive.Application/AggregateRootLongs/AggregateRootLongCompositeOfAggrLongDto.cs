using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRootLongs
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