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

    public class CreateCompositeSingleADTO : IMapFrom<CompositeSingleA>
    {
        public CreateCompositeSingleADTO()
        {
        }

        public static CreateCompositeSingleADTO Create(
            Guid id,
            string compositeAttr,
            CreateCompositeSingleAADTO? composite,
            List<CreateCompositeManyAADTO> composites)
        {
            return new CreateCompositeSingleADTO
            {
                Id = id,
                CompositeAttr = compositeAttr,
                Composite = composite,
                Composites = composites,
            };
        }

        public Guid Id { get; set; }

        public string CompositeAttr { get; set; }

        public CreateCompositeSingleAADTO? Composite { get; set; }

        public List<CreateCompositeManyAADTO> Composites { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CompositeSingleA, CreateCompositeSingleADTO>()
                .ForMember(d => d.Composites, opt => opt.MapFrom(src => src.Composites));
        }
    }
}