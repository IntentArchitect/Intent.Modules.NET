using System;
using Intent.RoslynWeaver.Attributes;

namespace Entities.PrivateSetters.EF.SqlServer.Domain.Entities
{
    public class Person
    {
        public Guid Id { get; protected set; }

        public string Name { get; protected set; }
    }
}