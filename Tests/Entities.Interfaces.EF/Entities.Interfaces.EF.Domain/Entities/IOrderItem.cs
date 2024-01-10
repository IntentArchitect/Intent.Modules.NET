using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace Entities.Interfaces.EF.Domain.Entities
{
    public interface IOrderItem
    {
        Guid Id { get; set; }

        string Description { get; set; }

        Guid OrderId { get; set; }
    }
}