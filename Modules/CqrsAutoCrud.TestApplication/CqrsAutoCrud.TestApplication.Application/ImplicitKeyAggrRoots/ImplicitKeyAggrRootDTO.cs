using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots
{

    public class ImplicitKeyAggrRootDTO : IMapFrom<ImplicitKeyAggrRoot>
    {
        public ImplicitKeyAggrRootDTO()
        {
        }

        public static ImplicitKeyAggrRootDTO Create(
            Guid id,
            string attribute,
            List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO> implicitKeyNestedCompositions)
        {
            return new ImplicitKeyAggrRootDTO
            {
                Id = id,
                Attribute = attribute,
                ImplicitKeyNestedCompositions = implicitKeyNestedCompositions,
            };
        }

        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO> ImplicitKeyNestedCompositions { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ImplicitKeyAggrRoot, ImplicitKeyAggrRootDTO>()
                .ForMember(d => d.ImplicitKeyNestedCompositions, opt => opt.MapFrom(src => src.ImplicitKeyNestedCompositions));
        }
    }
}