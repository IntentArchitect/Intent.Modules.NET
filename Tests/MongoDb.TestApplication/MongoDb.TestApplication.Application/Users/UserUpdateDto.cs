using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MongoDb.TestApplication.Application.Users
{

    public class UserUpdateDto
    {
        public UserUpdateDto()
        {
        }

        public static UserUpdateDto Create(
            long id,
            string username,
            List<UpdateUserRoleDto> roles)
        {
            return new UserUpdateDto
            {
                Id = id,
                Username = username,
                Roles = roles,
            };
        }

        public long Id { get; set; }

        public string Username { get; set; }

        public List<UpdateUserRoleDto> Roles { get; set; }

    }
}