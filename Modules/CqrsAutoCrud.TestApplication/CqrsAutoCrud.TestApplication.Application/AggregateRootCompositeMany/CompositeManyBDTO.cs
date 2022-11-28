using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.AggregateRootCompositeMany
{

    public class CompositeManyBDTO : IMapFrom<CompositeManyB>
    {
        public CompositeManyBDTO()
        {
        }

        public static CompositeManyBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aAggregaterootId,
            CompositeSingleBBDTO? composite,
            List<CompositeManyBBDTO> composites)
        {
            return new CompositeManyBDTO
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

        public CompositeSingleBBDTO? Composite { get; set; }

        public List<CompositeManyBBDTO> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyB, CompositeManyBDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}