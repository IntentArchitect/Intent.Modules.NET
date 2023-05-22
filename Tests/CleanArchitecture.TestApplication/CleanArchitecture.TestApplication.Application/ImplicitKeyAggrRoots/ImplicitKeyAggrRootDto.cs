using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{

    public class ImplicitKeyAggrRootDto : IMapFrom<ImplicitKeyAggrRoot>
    {
        public ImplicitKeyAggrRootDto()
        {
            Attribute = null!;
            ImplicitKeyNestedCompositions = null!;
        }

        public static ImplicitKeyAggrRootDto Create(
            Guid id,
            string attribute,
            List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto> implicitKeyNestedCompositions)
        {
            return new ImplicitKeyAggrRootDto
            {
                Id = id,
                Attribute = attribute,
                ImplicitKeyNestedCompositions = implicitKeyNestedCompositions
            };
        }

        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public List<ImplicitKeyAggrRootImplicitKeyNestedCompositionDto> ImplicitKeyNestedCompositions { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ImplicitKeyAggrRoot, ImplicitKeyAggrRootDto>()
                .ForMember(d => d.ImplicitKeyNestedCompositions, opt => opt.MapFrom(src => src.ImplicitKeyNestedCompositions));
        }
    }
}