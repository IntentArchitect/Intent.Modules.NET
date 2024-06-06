using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.Comprehensive.Application.Common.Mappings;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.AggregateRoots
{
    public class AggregateRootCompositeSingleADto : IMapFrom<CompositeSingleA>
    {
        public AggregateRootCompositeSingleADto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; set; }
        public Guid Id { get; set; }
        public AggregateRootCompositeSingleACompositeSingleAADto? Composite { get; set; }
        public List<AggregateRootCompositeSingleACompositeManyAADto> Composites { get; set; }

        public static AggregateRootCompositeSingleADto Create(
            string compositeAttr,
            Guid id,
            AggregateRootCompositeSingleACompositeSingleAADto? composite,
            List<AggregateRootCompositeSingleACompositeManyAADto> composites)
        {
            return new AggregateRootCompositeSingleADto
            {
                CompositeAttr = compositeAttr,
                Id = id,
                Composite = composite,
                Composites = composites
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleA, AggregateRootCompositeSingleADto>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}