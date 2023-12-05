using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional
{
    public class OneToManyDest
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OneToManyDest()
        {
            Attribute = null!;
            Owner = null!;
        }

        public OneToManyDest(string attribute)
        {
            Attribute = attribute;
        }

        public Guid Id { get; private set; }

        public Guid OwnerId { get; private set; }

        public string Attribute { get; private set; }

        public virtual OneToManySource Owner { get; private set; }

        public void Operation(string attribute)
        {
            Attribute = attribute;
        }
    }
}