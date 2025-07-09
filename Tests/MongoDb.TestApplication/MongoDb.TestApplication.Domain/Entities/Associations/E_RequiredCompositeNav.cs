using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    public class E_RequiredCompositeNav
    {
        public E_RequiredCompositeNav()
        {
            Id = null!;
            Attribute = null!;
            E_RequiredDependent = null!;
        }

        public string Id { get; set; }

        public string Attribute { get; set; }

        public E_RequiredDependent E_RequiredDependent { get; set; }
    }
}