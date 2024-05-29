using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Common.Behaviours;
using CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.UpdateConcreteClass;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.Inheritance.ConcreteClasses
{
    public class UpdateConcreteClassCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateConcreteClassCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateConcreteClassCommand testCommand)
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
            fixture.Customize<UpdateConcreteClassCommand>(comp => comp.With(x => x.ConcreteAttr, () => default));
            var testCommand = fixture.Create<UpdateConcreteClassCommand>();
            yield return new object[] { testCommand, "ConcreteAttr", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateConcreteClassCommand>(comp => comp.With(x => x.BaseAttr, () => default));
            testCommand = fixture.Create<UpdateConcreteClassCommand>();
            yield return new object[] { testCommand, "BaseAttr", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            UpdateConcreteClassCommand testCommand,
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

        private ValidationBehaviour<UpdateConcreteClassCommand, Unit> GetValidationBehaviour()
        {
            return new ValidationBehaviour<UpdateConcreteClassCommand, Unit>(new[] { new UpdateConcreteClassCommandValidator() });
        }
    }
}