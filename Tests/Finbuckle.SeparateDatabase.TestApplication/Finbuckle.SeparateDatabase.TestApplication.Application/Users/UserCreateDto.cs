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
        }

        public static UserCreateDto Create(
            string email,
            string username)
        {
            return new UserCreateDto
            {
                Email = email,
                Username = username,
            };
        }

        public string Email { get; set; }

        public string Username { get; set; }

    }
}