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

    public class CompositeSingleADTO : IMapFrom<CompositeSingleA>
    {
        public CompositeSingleADTO()
        {
        }

        public static CompositeSingleADTO Create(
            Guid id,
            string compositeAttr,
            CompositeSingleAADTO? composite,
            List<CompositeManyAADTO> composites)
        {
            return new CompositeSingleADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public CompositeSingleAADTO? Composite { get; set; }

        public List<CompositeManyAADTO> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleA, CompositeSingleADTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}