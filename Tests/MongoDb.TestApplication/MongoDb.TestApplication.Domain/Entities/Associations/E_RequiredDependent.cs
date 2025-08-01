using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace MongoDb.TestApplication.Domain.Entities.Associations
{
    public class E_RequiredDependent
    {
        public E_RequiredDependent()
        {
            Attribute = null!;
            E_RequiredCompositeNav = null!;
        }

        public string Attribute { get; set; }

        public E_RequiredCompositeNav E_RequiredCompositeNav { get; set; }
    }
}