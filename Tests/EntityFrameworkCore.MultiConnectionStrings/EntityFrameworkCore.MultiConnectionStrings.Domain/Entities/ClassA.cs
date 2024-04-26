using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace EntityFrameworkCore.MultiConnectionStrings.Domain.Entities
{
    public class ClassA
    {
        public Guid Id { get; set; }

        public string Message { get; set; }
    }
}