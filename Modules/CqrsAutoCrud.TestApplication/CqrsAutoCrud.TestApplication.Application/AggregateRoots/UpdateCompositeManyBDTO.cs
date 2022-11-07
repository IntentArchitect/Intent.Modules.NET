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

    public class UpdateCompositeManyBDTO : IMapFrom<CompositeManyB>
    {
        public UpdateCompositeManyBDTO()
        {
        }

        public static UpdateCompositeManyBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aAggregaterootId,
            UpdateCompositeSingleBBDTO? composite,
            List<UpdateCompositeManyBBDTO> composites)
        {
            return new UpdateCompositeManyBDTO
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

        public UpdateCompositeSingleBBDTO? Composite { get; set; }

        public List<UpdateCompositeManyBBDTO> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyB, UpdateCompositeManyBDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}