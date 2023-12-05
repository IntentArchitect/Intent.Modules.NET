using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional
{
    public class OneToOneDest
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OneToOneDest()
        {
            Attribute = null!;
            OneToOneSource = null!;
        }

        public OneToOneDest(string attribute)
        {
            Attribute = attribute;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual OneToOneSource OneToOneSource { get; private set; }

        public void Operation(string attribute)
        {
            Attribute = attribute;
        }
    }
}