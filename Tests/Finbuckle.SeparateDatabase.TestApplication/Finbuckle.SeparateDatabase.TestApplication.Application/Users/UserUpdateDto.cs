using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Finbuckle.SeparateDatabase.TestApplication.Application.Users
{

    public class UserUpdateDto
    {
        public UserUpdateDto()
        {
        }

        public static UserUpdateDto Create(
            Guid id,
            string email,
            string username)
        {
            return new UserUpdateDto
            {
                Id = id,
                Email = email,
                Username = username,
            };
        }

        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

    }
}