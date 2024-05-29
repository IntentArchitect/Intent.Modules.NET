using System;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class AggregateRootCompositeManyBCompositeSingleBBDto : IMapFrom<CompositeSingleBB>
    {
        public AggregateRootCompositeManyBCompositeSingleBBDto()
        {
            CompositeAttr = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }

        public static AggregateRootCompositeManyBCompositeSingleBBDto Create(string compositeAttr, Guid id)
        {
            return new AggregateRootCompositeManyBCompositeSingleBBDto
            {
                CompositeAttr = compositeAttr,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleBB, AggregateRootCompositeManyBCompositeSingleBBDto>();
        }
    }
}