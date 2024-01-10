using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Entities.Interfaces.EF.Application.Orders.DeleteOrder;
using Entities.Interfaces.EF.Domain.Common.Exceptions;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Tests.Orders
{
    public class DeleteOrderCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var existingEntity = fixture.Create<Order>();
            fixture.Customize<DeleteOrderCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteOrderCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesOrderFromRepository(
            DeleteOrderCommand testCommand,
            Order existingEntity)
        {
            // Arrange
            var orderRepository = Substitute.For<IOrderRepository>();
            orderRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<IOrder>(existingEntity));

            var sut = new DeleteOrderCommandHandler(orderRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            orderRepository.Received(1).Remove(Arg.Is<Order>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidOrderId_ReturnsNotFound()
        {
            // Arrange
            var orderRepository = Substitute.For<IOrderRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteOrderCommand>();
            orderRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<IOrder>(default));


            var sut = new DeleteOrderCommandHandler(orderRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}