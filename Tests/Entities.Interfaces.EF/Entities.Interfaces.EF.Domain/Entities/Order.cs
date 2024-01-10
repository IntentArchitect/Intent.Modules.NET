using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Interfaces.EF.Domain.Entities.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace Entities.Interfaces.EF.Domain.Entities
{
    public class Order : IOrder
    {
        public Guid Id { get; set; }

        public string RefNo { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        ICollection<IOrderItem> IOrder.OrderItems
        {
            get => OrderItems.CreateWrapper<IOrderItem, OrderItem>();
            set => OrderItems = value.Cast<OrderItem>().ToList();
        }
    }
}