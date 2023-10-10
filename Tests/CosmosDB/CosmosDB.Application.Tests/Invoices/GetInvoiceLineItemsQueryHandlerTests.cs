using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CosmosDB.Application.Invoices;
using CosmosDB.Application.Invoices.GetInvoiceLineItems;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetAllQueryHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Invoices
{
    public class GetInvoiceLineItemsQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetInvoiceLineItemsQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetInvoiceLineItemsQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            var existingOwnerEntity = fixture.Create<Invoice>();
            yield return new object[] { existingOwnerEntity };
            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);

            existingOwnerEntity = fixture.Create<Invoice>();
            yield return new object[] { existingOwnerEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesLineItems(Invoice existingOwnerEntity)
        {
            // Arrange
            var fixture = new Fixture();
            var testQuery = fixture.Create<GetInvoiceLineItemsQuery>();
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testQuery.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new GetInvoiceLineItemsQueryHandler(invoiceRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            InvoiceAssertions.AssertEquivalent(results, existingOwnerEntity.LineItems);
        }
    }
}