using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users
{
    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
            Email = null!;
            Name = null!;
            Surname = null!;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public Guid QuoteId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public static UserDto Create(Guid id, string email, Guid quoteId, string name, string surname)
        {
            return new UserDto
            {
                Id = id,
                Email = email,
                QuoteId = quoteId,
                Name = name,
                Surname = surname
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>();
        }
    }
}