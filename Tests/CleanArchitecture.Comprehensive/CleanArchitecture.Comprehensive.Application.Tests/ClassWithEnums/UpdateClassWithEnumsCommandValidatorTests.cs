using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums.UpdateClassWithEnums;
using CleanArchitecture.Comprehensive.Application.Common.Behaviours;
using CleanArchitecture.Comprehensive.Domain.Enums;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.ClassWithEnums
{
    public class UpdateClassWithEnumsCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateClassWithEnumsCommand testCommand)
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
            fixture.Customize<UpdateClassWithEnumsCommand>(comp => comp.With(x => x.EnumWithDefaultLiteral, () => (EnumWithDefaultLiteral)4));
            var testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "EnumWithDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateClassWithEnumsCommand>(comp => comp.With(x => x.EnumWithoutDefaultLiteral, () => (EnumWithoutDefaultLiteral)0));
            testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "EnumWithoutDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateClassWithEnumsCommand>(comp => comp.With(x => x.EnumWithoutDefaultLiteral, () => (EnumWithoutDefaultLiteral)4));
            testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "EnumWithoutDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateClassWithEnumsCommand>(comp => comp.With(x => x.EnumWithoutValues, () => (EnumWithoutValues)3));
            testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "EnumWithoutValues", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateClassWithEnumsCommand>(comp => comp.With(x => x.NullibleEnumWithDefaultLiteral, () => (EnumWithDefaultLiteral?)4));
            testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "NullibleEnumWithDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateClassWithEnumsCommand>(comp => comp.With(x => x.NullibleEnumWithoutDefaultLiteral, () => (EnumWithoutDefaultLiteral?)0));
            testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "NullibleEnumWithoutDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateClassWithEnumsCommand>(comp => comp.With(x => x.NullibleEnumWithoutDefaultLiteral, () => (EnumWithoutDefaultLiteral?)4));
            testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "NullibleEnumWithoutDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<UpdateClassWithEnumsCommand>(comp => comp.With(x => x.NullibleEnumWithoutValues, () => (EnumWithoutValues?)3));
            testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "NullibleEnumWithoutValues", "has a range of values which does not include" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            UpdateClassWithEnumsCommand testCommand,
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

        private ValidationBehaviour<UpdateClassWithEnumsCommand, Unit> GetValidationBehaviour()
        {
            return new ValidationBehaviour<UpdateClassWithEnumsCommand, Unit>(new[] { new UpdateClassWithEnumsCommandValidator() });
        }
    }
}