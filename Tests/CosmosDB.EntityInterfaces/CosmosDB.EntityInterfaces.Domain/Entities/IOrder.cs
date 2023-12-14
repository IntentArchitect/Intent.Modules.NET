using System;
using System.Collections.Generic;
using CosmosDB.EntityInterfaces.Domain.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace CosmosDB.EntityInterfaces.Domain.Entities
{
    public interface IOrder : IHasDomainEvent
    {
        string Id { get; set; }

        string WarehouseId { get; set; }

        string RefNo { get; set; }

        DateTime OrderDate { get; set; }

        ICollection<IOrderItem> OrderItems { get; set; }
    }
}