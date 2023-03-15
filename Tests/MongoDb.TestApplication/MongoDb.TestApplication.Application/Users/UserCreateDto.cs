using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Users
{

    public class UserCreateDto
    {
        public UserCreateDto()
        {
        }

        public static UserCreateDto Create(
            string username,
            List<CreateUserRoleDto> roles)
        {
            return new UserCreateDto
            {
                Username = username,
                Roles = roles,
            };
        }

        public string Username { get; set; }

        public List<CreateUserRoleDto> Roles { get; set; }

    }
}