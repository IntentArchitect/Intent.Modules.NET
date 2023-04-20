using System;
using System.Collections.Generic;
using AutoMapper;
using CleanArchitecture.TestApplication.Application.Common.Mappings;
using CleanArchitecture.TestApplication.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.EntityWithMutableOperations
{

    public class EntityWithMutableOperationDto : IMapFrom<EntityWithMutableOperation>
    {
        public EntityWithMutableOperationDto()
        {
        }

        public static EntityWithMutableOperationDto Create(Guid id, string name)
        {
            return new EntityWithMutableOperationDto
            {
                Id = id,
                Name = name
            };
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EntityWithMutableOperation, EntityWithMutableOperationDto>();
        }
    }
}