using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Users
{
    public class UserCreateDto
    {
        public UserCreateDto()
        {
            Email = null!;
            UserName = null!;
            Preferences = null!;
        }

        public string Email { get; set; }
        public string UserName { get; set; }
        public List<PreferenceDto> Preferences { get; set; }

        public static UserCreateDto Create(string email, string userName, List<PreferenceDto> preferences)
        {
            return new UserCreateDto
            {
                Email = email,
                UserName = userName,
                Preferences = preferences
            };
        }
    }
}