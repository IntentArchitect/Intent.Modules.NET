using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Application.Roles
{
    public class RoleUpdateDto
    {
        public RoleUpdateDto()
        {
            Name = null!;
            Priviledges = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<PriviledgeDto> Priviledges { get; set; }

        public static RoleUpdateDto Create(Guid id, string name, List<PriviledgeDto> priviledges)
        {
            return new RoleUpdateDto
            {
                Id = id,
                Name = name,
                Priviledges = priviledges
            };
        }
    }
}