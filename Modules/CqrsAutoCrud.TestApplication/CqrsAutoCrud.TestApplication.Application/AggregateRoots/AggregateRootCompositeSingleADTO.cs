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

    public class AggregateRootCompositeSingleADTO : IMapFrom<CompositeSingleA>
    {
        public AggregateRootCompositeSingleADTO()
        {
        }

        public static AggregateRootCompositeSingleADTO Create(
            Guid id,
            string compositeAttr,
            AggregateRootCompositeSingleACompositeSingleAADTO? composite,
            List<AggregateRootCompositeSingleACompositeManyAADTO> composites)
        {
            return new AggregateRootCompositeSingleADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public AggregateRootCompositeSingleACompositeSingleAADTO? Composite { get; set; }

        public List<AggregateRootCompositeSingleACompositeManyAADTO> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleA, AggregateRootCompositeSingleADTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}