using ObjectMapping.Strict.Application.Orders;
using ObjectMapping.Strict.Domain;
using ObjectMapping.Strict.Domain.Entities;
using Xunit;

namespace ObjectMapping.Strict.Application.Tests.Mappings
{
    /// <summary>
    /// One assertion per Mapping Shape (1-21) of the Object Mapper Golden Sample, plus the Strict
    /// runtime contract (R1.6). Every value is distinct and non-default so a wrong mapping expression
    /// cannot accidentally satisfy an assertion.
    /// </summary>
    public class OrderDtoMappingExtensionsTests
    {
        private static readonly Guid CustomerForeignKey = new("11111111-1111-1111-1111-111111111111");
        private static readonly Guid CustomerEntityId = new("22222222-2222-2222-2222-222222222222");
        private static readonly Guid CouponId = new("33333333-3333-3333-3333-333333333333");
        private static readonly Guid OrderLineId = new("44444444-4444-4444-4444-444444444444");
        private static readonly Guid PaymentMethodId = new("55555555-5555-5555-5555-555555555555");
        private static readonly Guid TagId = new("66666666-6666-6666-6666-666666666666");

        /// <summary>
        /// A fully populated aggregate. Every scalar differs from every other scalar of the same type.
        /// </summary>
        internal static Order CreateFullyPopulatedOrder()
        {
            return new Order
            {
                Id = new Guid("77777777-7777-7777-7777-777777777777"),
                OrderNumber = "ORD-1",
                Status = OrderStatus.Shipped,
                Notes = "Leave at reception",
                CustomerId = CustomerForeignKey,
                Customer = new Customer
                {
                    Id = CustomerEntityId,
                    Name = "Ada",
                    Tier = CustomerTier.Gold,
                    Address = new Address
                    {
                        Line1 = "1 Elm Street",
                        City = "Springfield",
                        PostalCode = "4001"
                    }
                },
                Coupon = new Coupon
                {
                    Id = CouponId,
                    Code = "SAVE17",
                    PercentOff = 17,
                    Kind = CouponKind.FixedAmount
                },
                OrderLines =
                [
                new OrderLine
                {
                    Id = OrderLineId,
                    ProductName = "Widget",
                    Quantity = 3
                }
                ],
                Tags =
                [
                new Tag
                {
                    Id = TagId,
                    Name = "Priority"
                }
                ],
                PaymentMethods =
                [
                new CardPayment
                {
                    Id = PaymentMethodId,
                    Label = "Visa ending 4242",
                    CardLast4 = "4242"
                }
                ]
            };
        }

        [Fact]
        public void MapToOrderDto_MapsFlatPrimitive_Shape1()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal("ORD-1", result.OrderNumber);
        }

        [Fact]
        public void MapToOrderDto_MapsNullableScalar_Shape2()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal("Leave at reception", result.Notes);
        }

        [Fact]
        public void MapToOrderDto_MapsNullableScalarAsNull_WhenSourceIsNull_Shape2()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();
            order.Notes = null;

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Null(result.Notes);
        }

        [Fact]
        public void MapToOrderDto_MapsThroughNonNullableNavigationToProperty_Shape3()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal("Ada", result.CustomerName);
        }

        [Fact]
        public void MapToOrderDto_MapsThroughNullableNavigationToNestedDto_Shape4()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.NotNull(result.Coupon);
            Assert.Equal(CouponId, result.Coupon.Id);
            Assert.Equal("SAVE17", result.Coupon.Code);
            Assert.Equal(17, result.Coupon.PercentOff);
            Assert.Equal(CouponKind.FixedAmount, result.Coupon.Kind);
        }

        [Fact]
        public void MapToOrderDto_MapsThroughNonNullableNavigationToNestedDto_Shape5()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal(CustomerEntityId, result.Customer.Id);
            Assert.Equal("Ada", result.Customer.Name);
            Assert.Equal(CustomerTier.Gold, result.Customer.Tier);
        }

        [Fact]
        public void MapToOrderDto_MapsCollectionOfNestedDtos_Shape6()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            var line = Assert.Single(result.Lines);
            Assert.Equal(OrderLineId, line.Id);
            Assert.Equal("Widget", line.ProductName);
            Assert.Equal(3, line.Quantity);
        }

        [Fact]
        public void MapToOrderDto_MapsLocalForeignKey_NotTheNavigationPrimaryKey_Shape7()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal(CustomerForeignKey, result.CustomerId);
            Assert.NotEqual(CustomerEntityId, result.CustomerId);
        }

        [Fact]
        public void MapToOrderDto_MapsPrimaryKeyThroughNullableNavigation_Shape8()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal(CouponId, result.CouponId);
            Assert.NotEqual(CustomerForeignKey, result.CouponId);
        }

        [Fact]
        public void MapToOrderDto_MapsPrimaryKeysThroughCollectionNavigation_Shape9()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal([OrderLineId], result.LineIds);
        }

        [Fact]
        public void MapToOrderDto_MapsTrailingPropertyThroughCollectionNavigation_Shape10()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal(["Widget"], result.ProductNames);
        }

        [Fact]
        public void MapToOrderDto_MapsEnumOfTheSameTypeUntouched_Shape11()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal(OrderStatus.Shipped, result.Status);
        }

        [Fact]
        public void MapToOrderDto_CastsEnumToTheContractEnum_Shape12()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal(OrderStatusDto.Shipped, result.StatusView);
            Assert.NotEqual(default, result.StatusView);
        }

        [Fact]
        public void MapToOrderDto_InvokesParameterlessOperation_Shape13()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal("Order ORD-1 [Shipped]", result.DisplayLabel);
        }

        [Fact]
        public void MapToCardPaymentDto_MapsInheritedMember_Shape14()
        {
            // Arrange
            var cardPayment = new CardPayment
            {
                Id = PaymentMethodId,
                Label = "Visa ending 4242",
                CardLast4 = "4242"
            };

            // Act
            var result = cardPayment.MapToCardPaymentDto();

            // Assert
            Assert.Equal(PaymentMethodId, result.Id);
            Assert.Equal("Visa ending 4242", result.Label);
        }

        [Fact]
        public void MapToCardPaymentDto_MapsDerivedMember_Shape15()
        {
            // Arrange
            var cardPayment = new CardPayment
            {
                Id = PaymentMethodId,
                Label = "Visa ending 4242",
                CardLast4 = "4242"
            };

            // Act
            var result = cardPayment.MapToCardPaymentDto();

            // Assert
            Assert.Equal("4242", result.CardLast4);
        }

        [Fact]
        public void MapToOrderDto_MapsBaseTypedCollectionContainingDerivedEntities_Shape16()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            var payment = Assert.Single(result.Payments);
            Assert.Equal(PaymentMethodId, payment.Id);
            Assert.Equal("Visa ending 4242", payment.Label);
        }

        [Fact]
        public void MapToOrderDto_MapsDeepChainAcrossANullableHop_Shape17()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal("Springfield", result.CustomerCity);
        }

        [Fact]
        public void MapToOrderDto_MapsValueTypeAcrossANullableHop_Shape18()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal(17, result.CouponPercentOff);
        }

        [Fact]
        public void MapToOrderDto_MapsEnumAcrossANullableHop_Shape19()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal(CouponKind.FixedAmount, result.CouponKind);
            Assert.NotEqual(default, result.CouponKind);
        }

        [Fact]
        public void MapToOrderDto_MapsNullableCollectionTarget_Shape20()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal(["Priority"], result.TagNames);
        }

        [Fact]
        public void MapToOrderDto_YieldsNullNotEmpty_WhenNullableCollectionTargetSourceIsNull_Shape20()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();
            order.Tags = null;

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Null(result.TagNames);
        }

        [Fact]
        public void MapToOrderDto_YieldsEmptyLists_WhenNonNullableCollectionTargetSourceIsNull()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();
            order.OrderLines = null;

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Empty(result.Lines);
            Assert.Empty(result.LineIds);
            Assert.Empty(result.ProductNames);
        }

        [Fact]
        public void MapToOrderDto_YieldsEmptyList_WhenNonNullablePaymentCollectionSourceIsNull()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();
            order.PaymentMethods = null;

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Empty(result.Payments);
        }

        [Fact]
        public void MapToOrderDto_LeavesUnmappedFieldAtItsClrDefault()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Null(result.UnmappedNote);
        }

        // Expressions are PascalCase (src.OrderNumber) because the Domain attributes are OrderNumber/Status, so PascalCasePropertyAccesses is a pass-through here, not an active casing conversion.
        [Fact]
        public void MapToOrderDto_ProducesByteIdenticalOutputForBothExpressionPrefixForms_Shape21()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();

            // Act
            var result = order.MapToOrderDto();

            // Assert
            Assert.Equal("ORD-1 / Shipped", result.SrcFormLabel);
            Assert.Equal("ORD-1 / Shipped", result.ProjectFromFormLabel);
            Assert.Equal(result.SrcFormLabel, result.ProjectFromFormLabel);
        }

        [Fact]
        public void MapToOrderDtoList_MapsEveryElement()
        {
            // Arrange
            var first = CreateFullyPopulatedOrder();
            var second = CreateFullyPopulatedOrder();
            second.OrderNumber = "ORD-2";

            // Act
            var result = new[] { first, second }.MapToOrderDtoList();

            // Assert
            Assert.Equal(["ORD-1", "ORD-2"], result.Select(x => x.OrderNumber));
        }

        [Fact]
        public void MapToOrderDto_Throws_WhenNullableHopIntoNonNullableTargetIsNull_Coupon()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();
            order.Coupon = null;

            // Act
            var exception = Record.Exception(() => order.MapToOrderDto());

            // Assert
            Assert.IsType<NullReferenceException>(exception);
        }

        [Fact]
        public void MapToOrderDto_Throws_WhenNullableHopIntoNonNullableTargetIsNull_Address()
        {
            // Arrange
            var order = CreateFullyPopulatedOrder();
            order.Customer.Address = null;

            // Act
            var exception = Record.Exception(() => order.MapToOrderDto());

            // Assert
            Assert.IsType<NullReferenceException>(exception);
        }
    }
}
