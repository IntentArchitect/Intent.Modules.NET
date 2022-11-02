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

    public class CompositeSingleAADTO : IMapFrom<CompositeSingleAA>
    {
        public CompositeSingleAADTO()
        {
        }

        public static CompositeSingleAADTO Create(
            Guid id,
            string compositeAttr,
            CompositeSingleAAA1DTO? composite,
            List<CompositeManyAAA1DTO> composites)
        {
            return new CompositeSingleAADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public CompositeSingleAAA1DTO? Composite { get; set; }

        public List<CompositeManyAAA1DTO> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleAA, CompositeSingleAADTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}