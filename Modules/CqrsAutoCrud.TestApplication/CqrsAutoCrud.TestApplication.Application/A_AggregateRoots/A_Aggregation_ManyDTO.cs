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

    public class A_Aggregation_ManyDTO : IMapFrom<A_Aggregation_Many>
    {
        public A_Aggregation_ManyDTO()
        {
        }

        public static A_Aggregation_ManyDTO Create(
            Guid id,
            string aggregationAttr,
            AA4_Composite_SingleDTO composite,
            List<AA4_Composite_ManyDTO> composites,
            AA4_Aggregation_SingleDTO aggregation,
            List<AA4_Aggregation_ManyDTO> aggregations)
        {
            return new A_Aggregation_ManyDTO
            {
                Id = id,
                AggregationAttr = aggregationAttr,
                Composite = composite,
                Composites = composites,
                Aggregation = aggregation,
                Aggregations = aggregations,
            };
        }

        public Guid Id { get; set; }

        public string AggregationAttr { get; set; }

        public AA4_Composite_SingleDTO Composite { get; set; }

        public List<AA4_Composite_ManyDTO> Composites { get; set; }

        public AA4_Aggregation_SingleDTO Aggregation { get; set; }

        public List<AA4_Aggregation_ManyDTO> Aggregations { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<A_Aggregation_Many, A_Aggregation_ManyDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites))
                .ForMember(d => d.Aggregations, opt => opt.MapFrom(src => src.Aggregations));
        }
    }
}