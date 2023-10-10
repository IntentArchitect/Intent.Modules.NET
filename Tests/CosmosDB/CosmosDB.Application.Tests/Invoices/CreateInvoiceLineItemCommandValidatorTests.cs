using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Common.Behaviours;
using CosmosDB.Application.Invoices.CreateInvoiceLineItem;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CosmosDB.Application.Tests.Invoices
{
    public class CreateInvoiceLineItemCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateInvoiceLineItemCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(CreateInvoiceLineItemCommand testCommand)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<string>();
            // Act
            var result = await validator.Handle(testCommand, () => Task.FromResult(expectedId), CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
        }

        public static IEnumerable<object[]> GetFailedResultTestData()
        {
            var fixture = new Fixture();
            fixture.Customize<CreateInvoiceLineItemCommand>(comp => comp.With(x => x.InvoiceId, () => default));
            var testCommand = fixture.Create<CreateInvoiceLineItemCommand>();
            yield return new object[] { testCommand, "InvoiceId", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateInvoiceLineItemCommand>(comp => comp.With(x => x.Description, () => default));
            testCommand = fixture.Create<CreateInvoiceLineItemCommand>();
            yield return new object[] { testCommand, "Description", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            CreateInvoiceLineItemCommand testCommand,
            string expectedPropertyName,
            string expectedPhrase)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<string>();
            // Act
            var act = async () => await validator.Handle(testCommand, () => Task.FromResult(expectedId), CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
            .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
        }

        private ValidationBehaviour<CreateInvoiceLineItemCommand, string> GetValidationBehaviour()
        {
            return new ValidationBehaviour<CreateInvoiceLineItemCommand, string>(new[] { new CreateInvoiceLineItemCommandValidator() });
        }
    }
}