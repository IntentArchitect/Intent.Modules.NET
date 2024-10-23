using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Common;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Domain.Entities
{
    public class Order : IHasDomainEvent
    {
        private List<OrderItem> _orderItems = [];

        public Order(string refNo, DateTime orderDate, IEnumerable<OrderItem> orderItems)
        {
            RefNo = refNo;
            OrderDate = orderDate;
            _orderItems = new List<OrderItem>(orderItems);
        }

        /// <summary>
        /// Required by Entity Framework.
        /// </summary>
        protected Order()
        {
            RefNo = null!;
        }

        public Guid Id { get; private set; }

        public string RefNo { get; private set; }

        public DateTime OrderDate { get; private set; }

        public virtual IReadOnlyCollection<OrderItem> OrderItems
        {
            get => _orderItems.AsReadOnly();
            private set => _orderItems = new List<OrderItem>(value);
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        public void UpdateOrderItems(IEnumerable<OrderItemUpdateDC> orderItemDetails)
        {
            UpdateHelper.CreateOrUpdateCollection<OrderItemUpdateDC, OrderItem>(
                _orderItems,
                orderItemDetails,
                (x, y) => x.Id == y.Id,
                (domain, dto) =>
                {
                    if (domain == null)
                    {
                        return new OrderItem(dto.ProductId, dto.Quantity, dto.Amount);
                    }
                    else
                    {
                        domain.UpdateDetails(dto.ProductId, dto.Quantity, dto.Amount);
                        return domain;
                    }
                }
                );
        }
    }
}