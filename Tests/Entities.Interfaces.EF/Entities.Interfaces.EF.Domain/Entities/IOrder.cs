using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace Entities.Interfaces.EF.Domain.Entities
{
    public interface IOrder
    {
        Guid Id { get; set; }

        string RefNo { get; set; }

        ICollection<IOrderItem> OrderItems { get; set; }
    }
}