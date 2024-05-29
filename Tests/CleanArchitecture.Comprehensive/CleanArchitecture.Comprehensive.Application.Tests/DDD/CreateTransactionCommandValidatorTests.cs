using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Common.Behaviours;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using CleanArchitecture.Comprehensive.Application.DDD;
using CleanArchitecture.Comprehensive.Application.DDD.CreateTransaction;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.DDD
{
    public class CreateTransactionCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateTransactionCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(CreateTransactionCommand testCommand)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            // Act
            var result = await validator.Handle(testCommand, () => Task.FromResult(Unit.Value), CancellationToken.None);

            // Assert
            result.Should().Be(Unit.Value);
        }

        public static IEnumerable<object[]> GetFailedResultTestData()
        {
            var fixture = new Fixture();
            fixture.Customize<CreateTransactionCommand>(comp => comp.With(x => x.Current, () => default));
            var testCommand = fixture.Create<CreateTransactionCommand>();
            yield return new object[] { testCommand, "Current", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateTransactionCommand>(comp => comp.With(x => x.Description, () => default));
            testCommand = fixture.Create<CreateTransactionCommand>();
            yield return new object[] { testCommand, "Description", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateTransactionCommand>(comp => comp.With(x => x.Account, () => default));
            testCommand = fixture.Create<CreateTransactionCommand>();
            yield return new object[] { testCommand, "Account", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            CreateTransactionCommand testCommand,
            string expectedPropertyName,
            string expectedPhrase)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            // Act
            var act = async () => await validator.Handle(testCommand, () => Task.FromResult(Unit.Value), CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
                .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
        }

        private ValidationBehaviour<CreateTransactionCommand, Unit> GetValidationBehaviour()
        {
            var validatorProvider = Substitute.For<IValidatorProvider>();
            validatorProvider.GetValidator<CreateMoneyDto>().Returns(c => new CreateMoneyDtoValidator());
            validatorProvider.GetValidator<CreateAccountDto>().Returns(c => new CreateAccountDtoValidator());
            return new ValidationBehaviour<CreateTransactionCommand, Unit>(new[] { new CreateTransactionCommandValidator(validatorProvider) });
        }
    }
}