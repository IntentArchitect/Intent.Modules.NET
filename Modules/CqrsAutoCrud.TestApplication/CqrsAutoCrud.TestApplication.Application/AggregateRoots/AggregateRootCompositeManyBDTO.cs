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

    public class AggregateRootCompositeManyBDTO : IMapFrom<CompositeManyB>
    {
        public AggregateRootCompositeManyBDTO()
        {
        }

        public static AggregateRootCompositeManyBDTO Create(
            Guid id,
            string compositeAttr,
            Guid aggregateRootId,
            DateTime? someDate,
            AggregateRootCompositeManyBCompositeSingleBBDTO? composite,
            List<AggregateRootCompositeManyBCompositeManyBBDTO> composites)
        {
            return new AggregateRootCompositeManyBDTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                AggregateRootId = aggregateRootId,
                SomeDate = someDate,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public Guid AggregateRootId { get; set; }

        public DateTime? SomeDate { get; set; }

        public AggregateRootCompositeManyBCompositeSingleBBDTO? Composite { get; set; }

        public List<AggregateRootCompositeManyBCompositeManyBBDTO> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeManyB, AggregateRootCompositeManyBDTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}