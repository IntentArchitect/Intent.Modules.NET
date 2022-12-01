using System;
using System.Collections.Generic;
using AutoMapper;
using CqrsAutoCrud.TestApplication.Application.Common.Mappings;
using CqrsAutoCrud.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRootImplicitKeyNestedCompositions
{

    public class ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO : IMapFrom<ImplicitKeyNestedComposition>
    {
        public ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO()
        {
        }

        public static ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO Create(
            Guid id,
            Guid implicitKeyAggrRootId,
            string attribute)
        {
            return new ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO
            {
                Id = id,
                ImplicitKeyAggrRootId = implicitKeyAggrRootId,
                Attribute = attribute,
            };
        }

        public Guid Id { get; set; }

        public Guid ImplicitKeyAggrRootId { get; set; }

        public string Attribute { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ImplicitKeyNestedComposition, ImplicitKeyAggrRootImplicitKeyNestedCompositionDTO>();
        }
    }
}