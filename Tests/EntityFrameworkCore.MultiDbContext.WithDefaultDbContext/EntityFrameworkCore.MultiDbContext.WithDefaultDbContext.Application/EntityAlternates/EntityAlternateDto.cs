using System;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.Common.Mappings;
using EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.WithDefaultDbContext.Application.EntityAlternates
{
    public class EntityAlternateDto : IMapFrom<EntityAlternate>
    {
        public EntityAlternateDto()
        {
            Message = null!;
        }

        public Guid Id { get; set; }
        public string Message { get; set; }

        public static EntityAlternateDto Create(Guid id, string message)
        {
            return new EntityAlternateDto
            {
                Id = id,
                Message = message
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<EntityAlternate, EntityAlternateDto>();
        }
    }
}