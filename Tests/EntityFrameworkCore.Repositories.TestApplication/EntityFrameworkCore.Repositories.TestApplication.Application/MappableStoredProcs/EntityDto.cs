using System;
using AutoMapper;
using EntityFrameworkCore.Repositories.TestApplication.Application.Common.Mappings;
using EntityFrameworkCore.Repositories.TestApplication.Domain.Contracts.MappableStoredProcs;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs
{
    public class EntityDto : IMapFrom<EntityRecord>
    {
        public EntityDto()
        {
            Name = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public static EntityDto Create(Guid id, string name)
        {
            return new EntityDto
            {
                Id = id,
                Name = name
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EntityRecord, EntityDto>();
        }
    }
}