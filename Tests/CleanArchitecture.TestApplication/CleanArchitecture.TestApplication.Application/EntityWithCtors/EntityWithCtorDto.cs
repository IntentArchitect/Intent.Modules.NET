using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithCtors
{

    public class EntityWithCtorDto : IMapFrom<EntityWithCtor>
    {
        public EntityWithCtorDto()
        {
        }

        public static EntityWithCtorDto Create(Guid id, string name)
        {
            return new EntityWithCtorDto
            {
                Id = id,
                Name = name
            };
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EntityWithCtor, EntityWithCtorDto>();
        }
    }
}