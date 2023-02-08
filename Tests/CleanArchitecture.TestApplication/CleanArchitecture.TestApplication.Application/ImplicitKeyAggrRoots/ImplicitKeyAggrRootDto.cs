using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots
{

    public class ImplicitKeyAggrRootDto : IMapFrom<ImplicitKeyAggrRoot>
    {
        public ImplicitKeyAggrRootDto()
        {
        }

        public static ImplicitKeyAggrRootDto Create(
            Guid id,
            Guid id,
            string attribute)
        {
            return new ImplicitKeyAggrRootDto
            {
                Id = id,
                Id = id,
                Attribute = attribute,
            };
        }

        public Guid Id { get; set; }

        public Guid Id { get; set; }

        public string Attribute { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ImplicitKeyAggrRoot, ImplicitKeyAggrRootDto>();
        }
    }
}