using Moq;
using ObjectMapping.Strict.Application.Common.Pagination;
using ObjectMapping.Strict.Application.Orders;
using ObjectMapping.Strict.Application.Orders.GetOrderById;
using ObjectMapping.Strict.Application.Orders.GetOrderOrNull;
using ObjectMapping.Strict.Application.Orders.GetOrders;
using ObjectMapping.Strict.Application.Orders.GetOrdersPaged;
using ObjectMapping.Strict.Application.Tests.Mappings;
using ObjectMapping.Strict.Domain.Common.Exceptions;
using ObjectMapping.Strict.Domain.Entities;
using ObjectMapping.Strict.Domain.Repositories;
using Xunit;

namespace ObjectMapping.Strict.Application.Tests.Orders
{
  /// <summary>
  /// The "D4 pair" for each of the four query handlers — reflection proving no AutoMapper mapper is
  /// injected (R3.5, R8.8), and a behavioural test proving the Mapping Method actually ran — plus the
  /// paged metadata pass-through journeys (R4.2, R4.4, R8.9).
  /// </summary>
  public class OrderQueryHandlerTests
  {
    private const string MapperTypeFullName = "AutoMapper.IMapper";

    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();

    public static TheoryData<Type> HandlerTypes =>
      [
      typeof(GetOrderByIdHandler),
      typeof(GetOrdersHandler),
      typeof(GetOrderOrNullHandler),
      typeof(GetOrdersPagedHandler)
      ];

    [Theory]
    [MemberData(nameof(HandlerTypes))]
    public void Handler_DeclaresNoAutoMapperDependency(Type handlerType)
    {
      // Arrange
      var constructors = handlerType.GetConstructors();

      // Act
      var parameterTypeNames = constructors
        .SelectMany(x => x.GetParameters())
        .Select(x => x.ParameterType.FullName)
        .ToList();

      // Assert
      Assert.DoesNotContain(MapperTypeFullName, parameterTypeNames);
      Assert.DoesNotContain(
        handlerType.GetFields(System.Reflection.BindingFlags.Instance
          | System.Reflection.BindingFlags.NonPublic
          | System.Reflection.BindingFlags.Public)
          .Select(x => x.FieldType.FullName),
        x => x == MapperTypeFullName);
    }

    [Fact]
    public async Task Handle_ReturnsFullyMappedDto_GetOrderById()
    {
      // Arrange
      var order = OrderDtoMappingExtensionsTests.CreateFullyPopulatedOrder();
      _orderRepositoryMock
        .Setup(x => x.FindByIdAsync(order.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(order);
      var handler = new GetOrderByIdHandler(_orderRepositoryMock.Object);

      // Act
      var result = await handler.Handle(new GetOrderById(order.Id), CancellationToken.None);

      // Assert
      AssertFullyMapped(result);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenOrderMissing_GetOrderById()
    {
      // Arrange
      var id = Guid.NewGuid();
      _orderRepositoryMock
        .Setup(x => x.FindByIdAsync(id, It.IsAny<CancellationToken>()))
        .ReturnsAsync((Order)null!);
      var handler = new GetOrderByIdHandler(_orderRepositoryMock.Object);

      // Act
      var exception = await Record.ExceptionAsync(() => handler.Handle(new GetOrderById(id), CancellationToken.None));

      // Assert
      Assert.IsType<NotFoundException>(exception);
    }

    [Fact]
    public async Task Handle_ReturnsFullyMappedDtos_GetOrders()
    {
      // Arrange
      var order = OrderDtoMappingExtensionsTests.CreateFullyPopulatedOrder();
      _orderRepositoryMock
        .Setup(x => x.FindAllAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync([order]);
      var handler = new GetOrdersHandler(_orderRepositoryMock.Object);

      // Act
      var result = await handler.Handle(new GetOrders(), CancellationToken.None);

      // Assert
      AssertFullyMapped(Assert.Single(result));
    }

    [Fact]
    public async Task Handle_ReturnsFullyMappedDto_GetOrderOrNull()
    {
      // Arrange
      var order = OrderDtoMappingExtensionsTests.CreateFullyPopulatedOrder();
      _orderRepositoryMock
        .Setup(x => x.FindByIdAsync(order.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(order);
      var handler = new GetOrderOrNullHandler(_orderRepositoryMock.Object);

      // Act
      var result = await handler.Handle(new GetOrderOrNull(order.Id), CancellationToken.None);

      // Assert
      Assert.NotNull(result);
      AssertFullyMapped(result);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenOrderMissing_GetOrderOrNull()
    {
      // Arrange
      var id = Guid.NewGuid();
      _orderRepositoryMock
        .Setup(x => x.FindByIdAsync(id, It.IsAny<CancellationToken>()))
        .ReturnsAsync((Order)null!);
      var handler = new GetOrderOrNullHandler(_orderRepositoryMock.Object);

      // Act
      var result = await handler.Handle(new GetOrderOrNull(id), CancellationToken.None);

      // Assert
      Assert.Null(result);
    }

    [Fact]
    public async Task Handle_MapsEveryItemAndPreservesPageMetadata_GetOrdersPaged()
    {
      // Arrange
      var order = OrderDtoMappingExtensionsTests.CreateFullyPopulatedOrder();
      var page = new PagedList<Order>(totalCount: 7, pageNo: 2, pageSize: 3, results: [order]);
      _orderRepositoryMock
        .Setup(x => x.FindAllAsync(2, 3, It.IsAny<CancellationToken>()))
        .ReturnsAsync(page);
      var handler = new GetOrdersPagedHandler(_orderRepositoryMock.Object);

      // Act
      var result = await handler.Handle(new GetOrdersPaged(2, 3), CancellationToken.None);

      // Assert
      Assert.Equal(7, result.TotalCount);
      Assert.Equal(2, result.PageNumber);
      Assert.Equal(3, result.PageSize);
      Assert.Equal(3, result.PageCount);
      AssertFullyMapped(Assert.Single(result.Data));
    }

    [Fact]
    public async Task Handle_PreservesPageMetadata_WhenPageIsEmpty_GetOrdersPaged()
    {
      // Arrange
      var page = new PagedList<Order>(totalCount: 0, pageNo: 1, pageSize: 10, results: []);
      _orderRepositoryMock
        .Setup(x => x.FindAllAsync(1, 10, It.IsAny<CancellationToken>()))
        .ReturnsAsync(page);
      var handler = new GetOrdersPagedHandler(_orderRepositoryMock.Object);

      // Act
      var result = await handler.Handle(new GetOrdersPaged(1, 10), CancellationToken.None);

      // Assert
      Assert.Empty(result.Data);
      Assert.Equal(0, result.TotalCount);
      Assert.Equal(1, result.PageNumber);
      Assert.Equal(10, result.PageSize);
    }

    /// <summary>
    /// Only a genuine invocation of the Mapping Method can populate all of these.
    /// </summary>
    private static void AssertFullyMapped(OrderDto dto)
    {
      Assert.Equal("ORD-1", dto.OrderNumber);
      Assert.Equal("Ada", dto.CustomerName);
      Assert.Equal("Springfield", dto.CustomerCity);
      Assert.Equal(17, dto.CouponPercentOff);
      Assert.Equal("Order ORD-1 [Shipped]", dto.DisplayLabel);
      Assert.NotNull(dto.Coupon);
      Assert.Single(dto.Lines);
      Assert.Single(dto.Payments);
      Assert.Equal(["Priority"], dto.TagNames);
    }
  }
}
