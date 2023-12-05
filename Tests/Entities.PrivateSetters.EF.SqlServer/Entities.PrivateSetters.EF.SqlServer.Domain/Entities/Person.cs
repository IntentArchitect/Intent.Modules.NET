using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Entities
{
    public class Person
    {
        public Guid Id { get; protected set; }

        public string Name { get; protected set; }
    }
}