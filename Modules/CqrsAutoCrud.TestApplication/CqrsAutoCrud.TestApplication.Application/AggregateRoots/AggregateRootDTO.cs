using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRoots
{

    public class AggregateRootDTO : IMapFrom<AggregateRoot>
    {
        public AggregateRootDTO()
        {
        }

        public static AggregateRootDTO Create(
            Guid id,
            string aggregateAttr,
            AggregateRootCompositeSingleADTO? composite,
            List<AggregateRootCompositeManyBDTO> composites,
            AggregateRootAggregateSingleCDTO? aggregate)
        {
            return new AggregateRootDTO
            {
                Id = id,
                AggregateAttr = aggregateAttr,
                Composite = composite,
                Composites = composites,
                Aggregate = aggregate,
            };
        }

        public Guid Id { get; set; }

        public string AggregateAttr { get; set; }

        public AggregateRootCompositeSingleADTO? Composite { get; set; }

        public List<AggregateRootCompositeManyBDTO> Composites { get; set; }

        public AggregateRootAggregateSingleCDTO? Aggregate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRoot, AggregateRootDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}