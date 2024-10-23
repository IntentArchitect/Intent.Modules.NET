using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Users
{
    public class UserUpdateDto
    {
        public UserUpdateDto()
        {
            Name = null!;
            Surname = null!;
            Email = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        public static UserUpdateDto Create(Guid id, string name, string surname, string email)
        {
            return new UserUpdateDto
            {
                Id = id,
                Name = name,
                Surname = surname,
                Email = email
            };
        }
    }
}