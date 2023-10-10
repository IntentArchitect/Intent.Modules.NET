using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using CosmosDB.Application.Invoices;
using CosmosDB.Application.Invoices.GetInvoiceLineItemById;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedGetByIdQueryHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Invoices
{
    public class GetInvoiceLineItemByIdQueryHandlerTests
    {
        private readonly IMapper _mapper;

        public GetInvoiceLineItemByIdQueryHandlerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
                config =>
                {
                    config.AddMaps(typeof(GetInvoiceLineItemByIdQueryHandler));
                });
            _mapper = mapperConfiguration.CreateMapper();
        }

        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<Invoice>();
            var expectedEntity = existingOwnerEntity.LineItems.First();
            fixture.Customize<GetInvoiceLineItemByIdQuery>(comp => comp
                .With(x => x.InvoiceId, existingOwnerEntity.Id)
                .With(x => x.Id, expectedEntity.Id));
            var testQuery = fixture.Create<GetInvoiceLineItemByIdQuery>();
            yield return new object[] { testQuery, existingOwnerEntity, expectedEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidQuery_RetrievesLineItem(
            GetInvoiceLineItemByIdQuery testQuery,
            Invoice existingOwnerEntity,
            LineItem existingEntity)
        {
            // Arrange
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testQuery.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new GetInvoiceLineItemByIdQueryHandler(invoiceRepository, _mapper);

            // Act
            var results = await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            InvoiceAssertions.AssertEquivalent(results, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdQuery_ThrowsNotFoundException()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            fixture.Customize<Invoice>(comp => comp.With(p => p.LineItems, new List<LineItem>()));
            var existingOwnerEntity = fixture.Create<Invoice>();
            fixture.Customize<GetInvoiceLineItemByIdQuery>(comp => comp
                .With(p => p.InvoiceId, existingOwnerEntity.Id));
            var testQuery = fixture.Create<GetInvoiceLineItemByIdQuery>();
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testQuery.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));


            var sut = new GetInvoiceLineItemByIdQueryHandler(invoiceRepository, _mapper);

            // Act
            var act = async () => await sut.Handle(testQuery, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}