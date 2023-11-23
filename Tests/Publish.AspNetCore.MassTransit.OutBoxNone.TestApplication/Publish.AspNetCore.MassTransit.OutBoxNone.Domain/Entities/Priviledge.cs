using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace Publish.AspNetCore.MassTransit.OutBoxNone.Domain.Entities
{
    public class Priviledge
    {
        public Guid Id { get; set; }

        public Guid RoleId { get; set; }

        public string Name { get; set; }
    }
}