using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    public class A_OptionalDependent
    {
        public A_OptionalDependent()
        {
            OptDepAttribute = null!;
        }

        public string OptDepAttribute { get; set; }
    }
}