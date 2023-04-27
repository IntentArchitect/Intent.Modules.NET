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

    public class ImplicitKeyAggrRootImplicitKeyNestedCompositionDto : IMapFrom<ImplicitKeyNestedComposition>
    {
        public ImplicitKeyAggrRootImplicitKeyNestedCompositionDto()
        {
        }

        public static ImplicitKeyAggrRootImplicitKeyNestedCompositionDto Create(string attribute, Guid id)
        {
            return new ImplicitKeyAggrRootImplicitKeyNestedCompositionDto
            {
                Attribute = attribute,
                Id = id
            };
        }

        public string Attribute { get; set; }

        public Guid Id { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ImplicitKeyNestedComposition, ImplicitKeyAggrRootImplicitKeyNestedCompositionDto>();
        }
    }
}