using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Domain.Entities.Compositional
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class OneToOneSource
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public OneToOneSource(string attribute, OneToOneDest oneToOneDest)
        {
            Attribute = attribute;
            OneToOneDest = oneToOneDest;
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected OneToOneSource()
        {
            Attribute = null!;
        }

        public Guid Id { get; private set; }

        public string Attribute { get; private set; }

        public virtual OneToOneDest OneToOneDest { get; private set; }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Operation(string attribute, OneToOneDest oneToOneDest)
        {
            Attribute = attribute;
            OneToOneDest = oneToOneDest;
        }
    }
}