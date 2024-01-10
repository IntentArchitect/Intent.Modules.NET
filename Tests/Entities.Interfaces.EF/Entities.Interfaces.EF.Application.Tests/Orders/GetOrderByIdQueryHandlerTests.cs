using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Entities.Interfaces.EF.Application.Orders;
using Entities.Interfaces.EF.Application.Orders.GetOrderById;
using Entities.Interfaces.EF.Domain.Common.Exceptions;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetByIdQueryHandlerTests", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Tests.Orders
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetOrderByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();

            var existingEntity = fixture.Create<Order>();
            fixture.Customize<GetOrderByIdQuery>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testQuery = fixture.Create<GetOrderByIdQuery>();
            yield return new object[] { testQuery, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesOrder(GetOrderByIdQuery testQuery, Order existingEntity)
        {
            // Arrange
            var orderRepository = Substitute.For<IOrderRepository>();
            orderRepository.FindByIdAsync(testQuery.Id, CancellationToken.None)!.Returns(Task.FromResult<IOrder>(existingEntity));


            var sut = new GetOrderByIdQueryHandler(orderRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            OrderAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            var query = fixture.Create<GetOrderByIdQuery>();
            var orderRepository = Substitute.For<IOrderRepository>();
            orderRepository.FindByIdAsync(query.Id, CancellationToken.None)!.Returns(Task.FromResult<IOrder>(default));

            var sut = new GetOrderByIdQueryHandler(orderRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}