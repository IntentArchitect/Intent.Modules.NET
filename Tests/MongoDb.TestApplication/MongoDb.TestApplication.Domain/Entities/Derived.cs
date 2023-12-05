using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities
{
    public class Derived : BaseType
    {
        public string DerivedAttribute { get; set; }
    }
}