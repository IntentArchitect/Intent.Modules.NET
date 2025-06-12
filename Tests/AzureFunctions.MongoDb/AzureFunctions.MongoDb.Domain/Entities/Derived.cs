using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AzureFunctions.MongoDb.Domain.Entities
{
    public class Derived : BaseType
    {
        public Derived()
        {
            DerivedAttribute = null!;
        }

        public string DerivedAttribute { get; set; }
    }
}