using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Common.Mappings;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Application.Users
{
    public class UserDto : IMapFrom<User>
    {
        public UserDto()
        {
            Email = null!;
            UserName = null!;
            Preferences = null!;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public List<PreferenceDto> Preferences { get; set; }

        public static UserDto Create(Guid id, string email, string userName, List<PreferenceDto> preferences)
        {
            return new UserDto
            {
                Id = id,
                Email = email,
                UserName = userName,
                Preferences = preferences
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                .ForMember(d => d.Preferences, opt => opt.MapFrom(src => src.Preferences));
        }
    }
}