using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Entities.Interfaces.EF.Application.Orders;
using Entities.Interfaces.EF.Application.Orders.UpdateOrder;
using Entities.Interfaces.EF.Domain.Common;
using Entities.Interfaces.EF.Domain.Common.Exceptions;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Tests.Orders
{
    public class UpdateOrderCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var existingEntity = fixture.Create<Order>();
            fixture.Customize<UpdateOrderCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateOrderCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateOrderCommand testCommand,
            Order existingEntity)
        {
            // Arrange
            var orderRepository = Substitute.For<IOrderRepository>();
            orderRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<IOrder>(existingEntity));

            var sut = new UpdateOrderCommandHandler(orderRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            OrderAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateOrderCommand>();
            var orderRepository = Substitute.For<IOrderRepository>();
            orderRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<IOrder>(default));


            var sut = new UpdateOrderCommandHandler(orderRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}