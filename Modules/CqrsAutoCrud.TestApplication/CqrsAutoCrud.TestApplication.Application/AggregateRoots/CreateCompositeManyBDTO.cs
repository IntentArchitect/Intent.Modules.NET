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

    public class CreateCompositeManyBDTO : IMapFrom<CompositeManyB>
    {
        public CreateCompositeManyBDTO()
        {
        }

        public static CreateCompositeManyBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aAggregaterootId,
            CreateCompositeSingleBBDTO? composite,
            List<CreateCompositeManyBBDTO> composites)
        {
            return new CreateCompositeManyBDTO
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

        public CreateCompositeSingleBBDTO? Composite { get; set; }

        public List<CreateCompositeManyBBDTO> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyB, CreateCompositeManyBDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}