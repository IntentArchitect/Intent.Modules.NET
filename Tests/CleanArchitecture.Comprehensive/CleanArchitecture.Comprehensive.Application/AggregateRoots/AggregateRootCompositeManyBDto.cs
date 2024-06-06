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
    public class AggregateRootCompositeManyBDto : IMapFrom<CompositeManyB>
    {
        public AggregateRootCompositeManyBDto()
        {
            CompositeAttr = null!;
            Composites = null!;
        }

        public string CompositeAttr { get; set; }
        public DateTime? SomeDate { get; set; }
        public Guid AggregateRootId { get; set; }
        public Guid Id { get; set; }
        public AggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }
        public List<AggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

        public static AggregateRootCompositeManyBDto Create(
            string compositeAttr,
            DateTime? someDate,
            Guid aggregateRootId,
            Guid id,
            AggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<AggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            return new AggregateRootCompositeManyBDto
            {
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                AggregateRootId = aggregateRootId,
                Id = id,
                Composite = composite,
                Composites = composites
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyB, AggregateRootCompositeManyBDto>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}