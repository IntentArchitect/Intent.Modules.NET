using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Finbuckle.SharedDatabase.TestApplication.Application.Users
{

    public class UserCreateDto
    {
        public UserCreateDto()
        {
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

        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public List<CreateUserRoleDto> Roles { get; set; } = null!;

    }
}