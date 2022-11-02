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

    public class A_Composite_ManyDTO : IMapFrom<A_Composite_Many>
    {
        public A_Composite_ManyDTO()
        {
        }

        public static A_Composite_ManyDTO Create(
            Guid id,
            string compositeAttr,
            AA2_Composite_SingleDTO composite,
            List<AA2_Composite_ManyDTO> composites,
            AA2_Aggregation_SingleDTO aggregation)
        {
            return new A_Composite_ManyDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites,
                Aggregation = aggregation,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public AA2_Composite_SingleDTO Composite { get; set; }

        public List<AA2_Composite_ManyDTO> Composites { get; set; }

        public AA2_Aggregation_SingleDTO Aggregation { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<A_Composite_Many, A_Composite_ManyDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}