using System;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Users
{
    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public Guid Id { get; set; }

        public static UserDto Create(string name, string surname, string email, Guid id)
        {
            return new UserDto
            {
                Name = name,
                Surname = surname,
                Email = email,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>();
        }
    }
}