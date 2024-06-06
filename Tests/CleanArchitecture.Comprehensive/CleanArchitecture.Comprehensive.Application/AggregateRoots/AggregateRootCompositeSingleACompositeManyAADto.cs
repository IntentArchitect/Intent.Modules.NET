using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class AggregateRootCompositeSingleACompositeManyAADto : IMapFrom<CompositeManyAA>
    {
        public AggregateRootCompositeSingleACompositeManyAADto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid CompositeSingleAId { get; set; }
        public Guid Id { get; set; }

        public static AggregateRootCompositeSingleACompositeManyAADto Create(
            string compositeAttr,
            Guid compositeSingleAId,
            Guid id)
        {
            return new AggregateRootCompositeSingleACompositeManyAADto
            {
                CompositeAttr = compositeAttr,
                CompositeSingleAId = compositeSingleAId,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyAA, AggregateRootCompositeSingleACompositeManyAADto>();
        }
    }
}