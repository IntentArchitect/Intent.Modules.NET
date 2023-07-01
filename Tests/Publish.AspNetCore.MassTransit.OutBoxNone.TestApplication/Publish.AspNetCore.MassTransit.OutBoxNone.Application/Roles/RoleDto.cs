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
    public class RoleDto : IMapFrom<Role>
    {
        public RoleDto()
        {
            Name = null!;
            Priviledges = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<PriviledgeDto> Priviledges { get; set; }

        public static RoleDto Create(Guid id, string name, List<PriviledgeDto> priviledges)
        {
            return new RoleDto
            {
                Id = id,
                Name = name,
                Priviledges = priviledges
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Role, RoleDto>()
                .ForMember(d => d.Priviledges, opt => opt.MapFrom(src => src.Priviledges));
        }
    }
}