using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.AggregateRoots
{

    public class AggregateRootDto : IMapFrom<AggregateRoot>
    {
        public AggregateRootDto()
        {
            AggregateAttr = null!;
            Composites = null!;
        }

        public static AggregateRootDto Create(
            Guid id,
            string aggregateAttr,
            List<AggregateRootCompositeManyBDto> composites,
            AggregateRootCompositeSingleADto? composite,
            AggregateRootAggregateSingleCDto? aggregate)
        {
            return new AggregateRootDto
            {
                Id = id,
                AggregateAttr = aggregateAttr,
                Composites = composites,
                Composite = composite,
                Aggregate = aggregate
            };
        }

        public Guid Id { get; set; }

        public string AggregateAttr { get; set; }

        public List<AggregateRootCompositeManyBDto> Composites { get; set; }

        public AggregateRootCompositeSingleADto? Composite { get; set; }

        public AggregateRootAggregateSingleCDto? Aggregate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRoot, AggregateRootDto>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}