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

    public class A_Composite_SingleDTO : IMapFrom<A_Composite_Single>
    {
        public A_Composite_SingleDTO()
        {
        }

        public static A_Composite_SingleDTO Create(
            Guid id,
            string compositeAttr,
            AA1_Composite_SingleDTO composite,
            List<AA1_Composite_ManyDTO> composites,
            AA1_Aggregation_SingleDTO aggregation)
        {
            return new A_Composite_SingleDTO
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

        public AA1_Composite_SingleDTO Composite { get; set; }

        public List<AA1_Composite_ManyDTO> Composites { get; set; }

        public AA1_Aggregation_SingleDTO Aggregation { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<A_Composite_Single, A_Composite_SingleDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}