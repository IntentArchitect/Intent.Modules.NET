using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootAS
{

    public class AggregateRootADTO : IMapFrom<AggregateRootA>
    {
        public AggregateRootADTO()
        {
        }

        public static AggregateRootADTO Create(
            Guid id,
            string aggregateAttr,
            CompositeSingleAADTO? composite,
            List<CompositeManyAADTO> composites,
            AggregateSingleAADTO? aggregate)
        {
            return new AggregateRootADTO
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

        public CompositeSingleAADTO? Composite { get; set; }

        public List<CompositeManyAADTO> Composites { get; set; }

        public AggregateSingleAADTO? Aggregate { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AggregateRootA, AggregateRootADTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}