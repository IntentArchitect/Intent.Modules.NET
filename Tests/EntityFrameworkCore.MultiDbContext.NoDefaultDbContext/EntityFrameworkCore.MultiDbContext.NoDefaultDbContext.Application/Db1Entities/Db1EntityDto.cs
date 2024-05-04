using System;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Mappings;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities
{
    public class Db1EntityDto : IMapFrom<Db1Entity>
    {
        public Db1EntityDto()
        {
            Message = null!;
        }

        public Guid Id { get; set; }
        public string Message { get; set; }

        public static Db1EntityDto Create(Guid id, string message)
        {
            return new Db1EntityDto
            {
                Id = id,
                Message = message
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Db1Entity, Db1EntityDto>();
        }
    }
}