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

    public class CompositeManyAADTO : IMapFrom<CompositeManyAA>
    {
        public CompositeManyAADTO()
        {
        }

        public static CompositeManyAADTO Create(
            Guid id,
            string compositeAttr,
            Guid aAggregaterootId,
            CompositeSingleAAA2DTO? composite,
            List<CompositeManyAAA2DTO> composites)
        {
            return new CompositeManyAADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                AAggregaterootId = aAggregaterootId,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid AAggregaterootId { get; set; }

        public CompositeSingleAAA2DTO? Composite { get; set; }

        public List<CompositeManyAAA2DTO> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyAA, CompositeManyAADTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}