using System;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace RichDomain.Domain.Entities
{
    public interface IAbstractBaseClass : IHasDomainEvent
    {
        Guid Id { get; }

        string AbstractBaseAttribute { get; }

        void BaseOperation(string abstractBaseAttribute);

        bool AbstractOp(string thing);
    }
}