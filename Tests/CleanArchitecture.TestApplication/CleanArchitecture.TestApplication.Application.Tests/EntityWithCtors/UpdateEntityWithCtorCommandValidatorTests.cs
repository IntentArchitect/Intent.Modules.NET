using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using CleanArchitecture.TestApplication.Application.EntityWithCtors.UpdateEntityWithCtor;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.EntityWithCtors
{
    public class UpdateEntityWithCtorCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateEntityWithCtorCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateEntityWithCtorCommand testCommand)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            // Act
            var result = await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(Unit.Value));

            // Assert
            result.Should().Be(Unit.Value);
        }

        public static IEnumerable<object[]> GetFailedResultTestData()
        {
            var fixture = new Fixture();
            fixture.Customize<UpdateEntityWithCtorCommand>(comp => comp.With(x => x.Name, () => default));
            var testCommand = fixture.Create<UpdateEntityWithCtorCommand>();
            yield return new object[] { testCommand, "Name", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(UpdateEntityWithCtorCommand testCommand, string expectedPropertyName, string expectedPhrase)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            // Act
            var act = async () => await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(Unit.Value));

            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
            .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
        }

        private ValidationBehaviour<UpdateEntityWithCtorCommand, Unit> GetValidationBehaviour()
        {
            return new ValidationBehaviour<UpdateEntityWithCtorCommand, Unit>(new[] { new UpdateEntityWithCtorCommandValidator() });
        }
    }
}