using System.Collections.Generic;
using System.Linq;
using Entities.Interfaces.EF.Application.Orders;
using Entities.Interfaces.EF.Application.Orders.CreateOrder;
using Entities.Interfaces.EF.Application.Orders.UpdateOrder;
using Entities.Interfaces.EF.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Tests.Orders
{
    public static class OrderAssertions
    {
        public static void AssertEquivalent(CreateOrderCommand expectedDto, Order actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.RefNo.Should().Be(expectedDto.RefNo);
            AssertEquivalent(expectedDto.OrderItems, actualEntity.OrderItems);
        }

        public static void AssertEquivalent(
            IEnumerable<CreateOrderOrderItemDto> expectedDtos,
            IEnumerable<OrderItem> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.Description.Should().Be(dto.Description);
            }
        }

        public static void AssertEquivalent(IEnumerable<OrderDto> actualDtos, IEnumerable<Order> expectedEntities)
        {
            if (expectedEntities == null)
            {
                actualDtos.Should().BeNullOrEmpty();
                return;
            }

            actualDtos.Should().HaveSameCount(actualDtos);
            for (int i = 0; i < expectedEntities.Count(); i++)
            {
                var entity = expectedEntities.ElementAt(i);
                var dto = actualDtos.ElementAt(i);
                if (entity == null)
                {
                    dto.Should().BeNull();
                    continue;
                }

                dto.Should().NotBeNull();
                dto.Id.Should().Be(entity.Id);
                dto.RefNo.Should().Be(entity.RefNo);
            }
        }

        public static void AssertEquivalent(OrderDto actualDto, Order expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.RefNo.Should().Be(expectedEntity.RefNo);
        }

        public static void AssertEquivalent(UpdateOrderCommand expectedDto, Order actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.RefNo.Should().Be(expectedDto.RefNo);
            AssertEquivalent(expectedDto.OrderItems, actualEntity.OrderItems);
        }

        public static void AssertEquivalent(
            IEnumerable<UpdateOrderOrderItemDto> expectedDtos,
            IEnumerable<OrderItem> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.Description.Should().Be(dto.Description);
                entity.OrderId.Should().Be(dto.OrderId);
            }
        }
    }
}