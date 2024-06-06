using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class AggregateRootCompositeManyBCompositeManyBBDto : IMapFrom<CompositeManyBB>
    {
        public AggregateRootCompositeManyBCompositeManyBBDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid CompositeManyBId { get; set; }
        public Guid Id { get; set; }

        public static AggregateRootCompositeManyBCompositeManyBBDto Create(
            string compositeAttr,
            Guid compositeManyBId,
            Guid id)
        {
            return new AggregateRootCompositeManyBCompositeManyBBDto
            {
                CompositeAttr = compositeAttr,
                CompositeManyBId = compositeManyBId,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyBB, AggregateRootCompositeManyBCompositeManyBBDto>();
        }
    }
}