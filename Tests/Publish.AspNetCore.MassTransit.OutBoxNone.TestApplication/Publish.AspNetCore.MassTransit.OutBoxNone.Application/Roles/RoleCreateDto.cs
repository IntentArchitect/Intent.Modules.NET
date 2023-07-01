using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Application.Roles
{
    public class RoleCreateDto
    {
        public RoleCreateDto()
        {
            Name = null!;
            Priviledges = null!;
        }

        public string Name { get; set; }
        public List<PriviledgeDto> Priviledges { get; set; }

        public static RoleCreateDto Create(string name, List<PriviledgeDto> priviledges)
        {
            return new RoleCreateDto
            {
                Name = name,
                Priviledges = priviledges
            };
        }
    }
}