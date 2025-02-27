using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainInvoke
{
    public class Machines
    {
        public Machines()
        {
            Name = null!;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid FarmerId { get; set; }

        public void ChangeName(string name)
        {
            Name = name;
        }
    }
}