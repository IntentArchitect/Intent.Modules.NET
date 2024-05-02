using System;
using AutoMapper;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Common.Mappings;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities
{
    public class Db2EntityDto : IMapFrom<Db2Entity>
    {
        public Db2EntityDto()
        {
            Message = null!;
        }

        public Guid Id { get; set; }
        public string Message { get; set; }

        public static Db2EntityDto Create(Guid id, string message)
        {
            return new Db2EntityDto
            {
                Id = id,
                Message = message
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Db2Entity, Db2EntityDto>();
        }
    }
}