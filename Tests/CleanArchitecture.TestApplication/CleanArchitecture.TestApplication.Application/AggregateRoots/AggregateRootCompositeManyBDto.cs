using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class AggregateRootCompositeManyBDto : IMapFrom<CompositeManyB>
    {
        public AggregateRootCompositeManyBDto()
        {
        }

        public static AggregateRootCompositeManyBDto Create(
            Guid id,
            string compositeAttr,
            DateTime? someDate,
            Guid aggregateRootId,
            string compositeAttr,
            DateTime? someDate,
            Guid aggregateRootId,
            Guid id,
            AggregateRootCompositeManyBCompositeSingleBBDto? composite,
            List<AggregateRootCompositeManyBCompositeManyBBDto> composites)
        {
            return new AggregateRootCompositeManyBDto
            {
                Id = id,
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                AggregateRootId = aggregateRootId,
                CompositeAttr = compositeAttr,
                SomeDate = someDate,
                AggregateRootId = aggregateRootId,
                Id = id,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public DateTime? SomeDate { get; set; }

        public Guid AggregateRootId { get; set; }

        public string CompositeAttr { get; set; }

        public DateTime? SomeDate { get; set; }

        public Guid AggregateRootId { get; set; }

        public Guid Id { get; set; }

        public AggregateRootCompositeManyBCompositeSingleBBDto? Composite { get; set; }

        public List<AggregateRootCompositeManyBCompositeManyBBDto> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyB, AggregateRootCompositeManyBDto>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}