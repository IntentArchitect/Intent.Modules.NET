using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Invoices.DeleteInvoiceLineItem;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedDeleteCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Invoices
{
    public class DeleteInvoiceLineItemCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<Invoice>();
            var existingEntity = existingOwnerEntity.LineItems.First();
            fixture.Customize<DeleteInvoiceLineItemCommand>(comp => comp
                .With(x => x.InvoiceId, existingOwnerEntity.Id)
                .With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteInvoiceLineItemCommand>();
            yield return new object[] { testCommand, existingOwnerEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesLineItemFromInvoice(
            DeleteInvoiceLineItemCommand testCommand,
            Invoice existingOwnerEntity)
        {
            // Arrange
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testCommand.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new DeleteInvoiceLineItemCommandHandler(invoiceRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            existingOwnerEntity.LineItems.Should().NotContain(p => testCommand.Id == p.Id);
        }

        [Fact]
        public async Task Handle_WithInvalidInvoiceId_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteInvoiceLineItemCommand>();
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testCommand.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult<Invoice>(default));

            var sut = new DeleteInvoiceLineItemCommandHandler(invoiceRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WithInvalidLineItemId_ReturnsNotFound()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            fixture.Customize<Invoice>(comp => comp.With(p => p.LineItems, new List<LineItem>()));
            var existingOwnerEntity = fixture.Create<Invoice>();
            fixture.Customize<DeleteInvoiceLineItemCommand>(comp => comp
                .With(p => p.InvoiceId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<DeleteInvoiceLineItemCommand>();
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testCommand.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new DeleteInvoiceLineItemCommandHandler(invoiceRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}