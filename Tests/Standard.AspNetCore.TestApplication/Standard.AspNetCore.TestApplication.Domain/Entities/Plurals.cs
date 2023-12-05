using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Domain.Entities
{
    public class Plurals
    {
        public Guid Id { get; set; }
    }
}