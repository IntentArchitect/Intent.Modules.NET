using System;
using System.Collections.Generic;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Common;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Contracts;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities
{
    public partial class Order
    {
        public Order(string refNo, DateTime orderDate, IEnumerable<OrderItem> orderItems)
        {
            RefNo = refNo;
            OrderDate = orderDate;
            OrderItems = new List<OrderItem>(orderItems);
        }

        public void UpdateOrderItems(IEnumerable<OrderItemUpdateDC> orderItemDetails)
        {
            UpdateHelper.CreateOrUpdateCollection<OrderItemUpdateDC, OrderItem>(
                            OrderItems,
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