using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Invoices.DeleteInvoice;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.Invoices
{
    public class DeleteInvoiceCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<Invoice>();
            fixture.Customize<DeleteInvoiceCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteInvoiceCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesInvoiceFromRepository(
            DeleteInvoiceCommand testCommand,
            Invoice existingEntity)
        {
            // Arrange
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            invoiceRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteInvoiceCommandHandler(invoiceRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            invoiceRepository.Received(1).Remove(Arg.Is<Invoice>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidInvoiceId_ReturnsNotFound()
        {
            // Arrange
            var invoiceRepository = Substitute.For<IInvoiceRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteInvoiceCommand>();
            invoiceRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<Invoice>(default));


            var sut = new DeleteInvoiceCommandHandler(invoiceRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}