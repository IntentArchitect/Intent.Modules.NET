using System;
using System.Collections.Generic;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxNone.Application.Common.Mappings;
using Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Application.Roles
{
    public class PriviledgeDto : IMapFrom<Priviledge>
    {
        public PriviledgeDto()
        {
            Name = null!;
        }

        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }

        public static PriviledgeDto Create(Guid roleId, string name, Guid id)
        {
            return new PriviledgeDto
            {
                RoleId = roleId,
                Name = name,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Priviledge, PriviledgeDto>();
        }
    }
}