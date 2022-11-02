using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.A_AggregateRoots
{

    public class A_AggregateRootDTO : IMapFrom<A_AggregateRoot>
    {
        public A_AggregateRootDTO()
        {
        }

        public static A_AggregateRootDTO Create(
            Guid id,
            string aggregateAttr,
            A_Composite_SingleDTO composite,
            List<A_Composite_ManyDTO> composites,
            A_Aggregation_SingleDTO aggregation,
            List<A_Aggregation_ManyDTO> aggregations)
        {
            return new A_AggregateRootDTO
            {
                Id = id,
                AggregateAttr = aggregateAttr,
                Composite = composite,
                Composites = composites,
                Aggregation = aggregation,
                Aggregations = aggregations,
            };
        }

        public Guid Id { get; set; }

        public string AggregateAttr { get; set; }

        public A_Composite_SingleDTO Composite { get; set; }

        public List<A_Composite_ManyDTO> Composites { get; set; }

        public A_Aggregation_SingleDTO Aggregation { get; set; }

        public List<A_Aggregation_ManyDTO> Aggregations { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<A_AggregateRoot, A_AggregateRootDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites))
                .ForMember(d => d.Aggregations, opt => opt.MapFrom(src => src.Aggregations));
        }
    }
}