using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Domain.Entities.OperationAndConstructorMapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Properties)]
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods | Targets.Constructors, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class OpAndCtorMapping1
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        [IntentManaged(Mode.Fully)]
        protected OpAndCtorMapping1()
        {
            OpAndCtorMapping3 = null!;
            OpAndCtorMapping2 = null!;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public OpAndCtorMapping1(OpAndCtorMapping3 opAndCtorMapping3, OpAndCtorMapping2 opAndCtorMapping2)
        {
            OpAndCtorMapping3 = opAndCtorMapping3;
            OpAndCtorMapping2 = opAndCtorMapping2;
        }

        public Guid Id { get; set; }

        public Guid OpAndCtorMapping3Id { get; set; }

        public virtual OpAndCtorMapping3 OpAndCtorMapping3 { get; set; }

        public virtual OpAndCtorMapping2 OpAndCtorMapping2 { get; set; }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void Operation(OpAndCtorMapping3 opAndCtorMapping3, OpAndCtorMapping2 opAndCtorMapping2)
        {
            OpAndCtorMapping3 = opAndCtorMapping3;
            OpAndCtorMapping2 = opAndCtorMapping2;
        }
    }
}