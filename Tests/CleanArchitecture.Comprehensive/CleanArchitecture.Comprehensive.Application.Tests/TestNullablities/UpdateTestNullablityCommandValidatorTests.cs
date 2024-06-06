using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Common.Behaviours;
using CleanArchitecture.Comprehensive.Application.TestNullablities.UpdateTestNullablity;
using CleanArchitecture.Comprehensive.Domain.Nullability;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.TestNullablities
{
    public class UpdateTestNullablityCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateTestNullablityCommand testCommand)
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
            fixture.Customize<UpdateTestNullablityCommand>(comp => comp.With(x => x.SampleEnum, () => (NoDefaultLiteralEnum)0));
            var testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand, "SampleEnum", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateTestNullablityCommand>(comp => comp.With(x => x.SampleEnum, () => (NoDefaultLiteralEnum)3));
            testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand, "SampleEnum", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateTestNullablityCommand>(comp => comp.With(x => x.Str, () => default));
            testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand, "Str", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateTestNullablityCommand>(comp => comp.With(x => x.NullableEnum, () => (NoDefaultLiteralEnum?)0));
            testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand, "NullableEnum", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateTestNullablityCommand>(comp => comp.With(x => x.NullableEnum, () => (NoDefaultLiteralEnum?)3));
            testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand, "NullableEnum", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateTestNullablityCommand>(comp => comp.With(x => x.DefaultLiteralEnum, () => (DefaultLiteralEnum)3));
            testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand, "DefaultLiteralEnum", "has a range of values which does not include" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            UpdateTestNullablityCommand testCommand,
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

        private ValidationBehaviour<UpdateTestNullablityCommand, Unit> GetValidationBehaviour()
        {
            return new ValidationBehaviour<UpdateTestNullablityCommand, Unit>(new[] { new UpdateTestNullablityCommandValidator() });
        }
    }
}