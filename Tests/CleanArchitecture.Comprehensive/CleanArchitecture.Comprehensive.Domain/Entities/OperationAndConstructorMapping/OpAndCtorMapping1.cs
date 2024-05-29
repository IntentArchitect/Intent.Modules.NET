using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Domain.Entities.OperationAndConstructorMapping
{
    public class OpAndCtorMapping1
    {
        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected OpAndCtorMapping1()
        {
            OpAndCtorMapping3 = null!;
            OpAndCtorMapping2 = null!;
        }

        public OpAndCtorMapping1(OpAndCtorMapping3 opAndCtorMapping3, OpAndCtorMapping2 opAndCtorMapping2)
        {
            OpAndCtorMapping3 = opAndCtorMapping3;
            OpAndCtorMapping2 = opAndCtorMapping2;
        }

        public Guid Id { get; set; }

        public Guid OpAndCtorMapping3Id { get; set; }

        public virtual OpAndCtorMapping3 OpAndCtorMapping3 { get; set; }

        public virtual OpAndCtorMapping2 OpAndCtorMapping2 { get; set; }

        public void Operation(OpAndCtorMapping3 opAndCtorMapping3, OpAndCtorMapping2 opAndCtorMapping2)
        {
            OpAndCtorMapping3 = opAndCtorMapping3;
            OpAndCtorMapping2 = opAndCtorMapping2;
        }
    }
}