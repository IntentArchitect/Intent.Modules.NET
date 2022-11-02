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

    public class A_Aggregation_SingleDTO : IMapFrom<A_Aggregation_Single>
    {
        public A_Aggregation_SingleDTO()
        {
        }

        public static A_Aggregation_SingleDTO Create(
            Guid id,
            string aggregationAttr,
            AA3_Composite_SingleDTO composite,
            List<AA3_Composite_ManyDTO> composites,
            AA3_Aggregation_SingleDTO aggregation,
            List<AA3_Aggregation_ManyDTO> aggregations)
        {
            return new A_Aggregation_SingleDTO
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

        public AA3_Composite_SingleDTO Composite { get; set; }

        public List<AA3_Composite_ManyDTO> Composites { get; set; }

        public AA3_Aggregation_SingleDTO Aggregation { get; set; }

        public List<AA3_Aggregation_ManyDTO> Aggregations { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<A_Aggregation_Single, A_Aggregation_SingleDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites))
                .ForMember(d => d.Aggregations, opt => opt.MapFrom(src => src.Aggregations));
        }
    }
}