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
    public class PriviledgeDto
    {
        public PriviledgeDto()
        {
            Name = null!;
        }

        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }

        public static PriviledgeDto Create(Guid id, Guid roleId, string name)
        {
            return new PriviledgeDto
            {
                Id = id,
                RoleId = roleId,
                Name = name
            };
        }
    }
}