using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Users
{

    public class UserCreateDto
    {
        public UserCreateDto()
        {
            Email = null!;
            Username = null!;
            Roles = null!;
        }

        public static UserCreateDto Create(string email, string username, List<CreateUserRoleDto> roles)
        {
            return new UserCreateDto
            {
                Email = email,
                Username = username,
                Roles = roles
            };
        }

        public string Email { get; set; }

        public string Username { get; set; }

        public List<CreateUserRoleDto> Roles { get; set; }

    }
}