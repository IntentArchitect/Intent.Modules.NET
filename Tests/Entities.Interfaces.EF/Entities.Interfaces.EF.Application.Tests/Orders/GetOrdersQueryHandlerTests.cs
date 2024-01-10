using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using Entities.Interfaces.EF.Application.Orders;
using Entities.Interfaces.EF.Application.Orders.GetOrders;
using Entities.Interfaces.EF.Domain.Entities;
using Entities.Interfaces.EF.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.GetAllQueryHandlerTests", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.Tests.Orders
{
    public class GetOrdersQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetOrdersQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetOrdersQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.CreateMany<Order>().ToList() };
            yield return new object[] { fixture.CreateMany<Order>(0).ToList() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesOrders(List<Order> testEntities)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetOrdersQuery>();
            var orderRepository = Substitute.For<IOrderRepository>();
            orderRepository.FindAllAsync(CancellationToken.None).Returns(Task.FromResult(testEntities.Cast<IOrder>().ToList()));

            var sut = new GetOrdersQueryHandler(orderRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            OrderAssertions.AssertEquivalent(results, testEntities);
        }
    }
}