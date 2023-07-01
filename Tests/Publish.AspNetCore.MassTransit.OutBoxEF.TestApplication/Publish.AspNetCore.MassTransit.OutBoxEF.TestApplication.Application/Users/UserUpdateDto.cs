using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Users
{
    public class UserUpdateDto
    {
        public UserUpdateDto()
        {
            Email = null!;
            UserName = null!;
            Preferences = null!;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public List<PreferenceDto> Preferences { get; set; }

        public static UserUpdateDto Create(Guid id, string email, string userName, List<PreferenceDto> preferences)
        {
            return new UserUpdateDto
            {
                Id = id,
                Email = email,
                UserName = userName,
                Preferences = preferences
            };
        }
    }
}