using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional
{
    public class OneToOneSource
    {
        public OneToOneSource(string attribute, OneToOneDest oneToOneDest)
        {
            Attribute = attribute;
            OneToOneDest = oneToOneDest;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OneToOneSource()
        {
            Attribute = null!;
            OneToOneDest = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual OneToOneDest OneToOneDest { get; private set; }

        public void Operation(string attribute, OneToOneDest oneToOneDest)
        {
            Attribute = attribute;
            OneToOneDest = oneToOneDest;
        }
    }
}