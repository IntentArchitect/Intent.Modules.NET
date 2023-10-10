using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Invoices.CreateInvoiceLineItem;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedCreateCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Invoices
{
    public class CreateInvoiceLineItemCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<Invoice>();
            var existingEntity = existingOwnerEntity.LineItems.First();
            fixture.Customize<CreateInvoiceLineItemCommand>(comp => comp
                .With(x => x.InvoiceId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<CreateInvoiceLineItemCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsLineItemToInvoice(
            CreateInvoiceLineItemCommand testCommand,
            Invoice existingOwnerEntity)
        {
            // Arrange
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testCommand.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));
            var expectedInvoiceId = new Fixture().Create<string>();
            LineItem addedLineItem = null;
            var lineItemsSnapshot = existingOwnerEntity.LineItems.ToArray();
            invoiceRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ =>
                {
                    addedLineItem = existingOwnerEntity.LineItems.Except(lineItemsSnapshot).Single();
                    addedLineItem.Id = expectedInvoiceId;
                });

            var sut = new CreateInvoiceLineItemCommandHandler(invoiceRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedInvoiceId);
            await invoiceRepository.UnitOfWork.Received(1).SaveChangesAsync();
            InvoiceAssertions.AssertEquivalent(testCommand, addedLineItem);
        }
    }
}