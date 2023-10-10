using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Invoices.UpdateInvoiceLineItem;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedUpdateCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Invoices
{
    public class UpdateInvoiceLineItemCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<Invoice>();
            var existingEntity = existingOwnerEntity.LineItems.First();
            fixture.Customize<UpdateInvoiceLineItemCommand>(comp => comp
                .With(x => x.InvoiceId, existingOwnerEntity.Id)
                .With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateInvoiceLineItemCommand>();
            yield return new object[] { testCommand, existingOwnerEntity, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesLineItem(
            UpdateInvoiceLineItemCommand testCommand,
            Invoice existingOwnerEntity,
            LineItem existingEntity)
        {
            // Arrange
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testCommand.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new UpdateInvoiceLineItemCommandHandler(invoiceRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            InvoiceAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidInvoiceId_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateInvoiceLineItemCommand>();
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testCommand.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult<Invoice>(default));

            var sut = new UpdateInvoiceLineItemCommandHandler(invoiceRepository);

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
            fixture.Customize<UpdateInvoiceLineItemCommand>(comp => comp
                .With(p => p.InvoiceId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<UpdateInvoiceLineItemCommand>();
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testCommand.InvoiceId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new UpdateInvoiceLineItemCommandHandler(invoiceRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}