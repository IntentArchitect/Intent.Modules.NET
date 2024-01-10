using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Entities.Interfaces.EF.Application.Orders;
using Entities.Interfaces.EF.Application.Orders.CreateOrder;
using Entities.Interfaces.EF.Application.Tests.Extensions;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Tests.Orders
{
    public class CreateOrderCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateOrderCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsOrderToRepository(CreateOrderCommand testCommand)
        {
            // Arrange
            var orderRepository = Substitute.For<IOrderRepository>();
            var expectedOrderId = new Fixture().Create<System.Guid>();
            Order addedOrder = null;
            orderRepository.OnAdd(ent => addedOrder = (Order)ent);
            orderRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedOrder.Id = expectedOrderId);

            var sut = new CreateOrderCommandHandler(orderRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedOrderId);
            await orderRepository.UnitOfWork.Received(1).SaveChangesAsync();
            OrderAssertions.AssertEquivalent(testCommand, addedOrder);
        }
    }
}