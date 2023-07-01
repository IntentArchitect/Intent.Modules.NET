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
    public class PreferenceDto : IMapFrom<Preference>
    {
        public PreferenceDto()
        {
            Key = null!;
            Value = null!;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public Guid UserId { get; set; }
        public Guid Id { get; set; }

        public static PreferenceDto Create(string key, string value, Guid userId, Guid id)
        {
            return new PreferenceDto
            {
                Key = key,
                Value = value,
                UserId = userId,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Preference, PreferenceDto>();
        }
    }
}