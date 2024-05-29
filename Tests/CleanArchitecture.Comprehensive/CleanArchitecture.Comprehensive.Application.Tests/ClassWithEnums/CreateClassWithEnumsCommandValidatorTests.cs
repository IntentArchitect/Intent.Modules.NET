using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums.CreateClassWithEnums;
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
    public class CreateClassWithEnumsCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(CreateClassWithEnumsCommand testCommand)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<System.Guid>();
            // Act
            var result = await validator.Handle(testCommand, () => Task.FromResult(expectedId), CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
        }

        public static IEnumerable<object[]> GetFailedResultTestData()
        {
            var fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.EnumWithDefaultLiteral, () => (EnumWithDefaultLiteral)4));
            var testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "EnumWithDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.EnumWithoutDefaultLiteral, () => (EnumWithoutDefaultLiteral)0));
            testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "EnumWithoutDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.EnumWithoutDefaultLiteral, () => (EnumWithoutDefaultLiteral)4));
            testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "EnumWithoutDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.EnumWithoutValues, () => (EnumWithoutValues)3));
            testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "EnumWithoutValues", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.NullibleEnumWithDefaultLiteral, () => (EnumWithDefaultLiteral?)4));
            testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "NullibleEnumWithDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.NullibleEnumWithoutDefaultLiteral, () => (EnumWithoutDefaultLiteral?)0));
            testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "NullibleEnumWithoutDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.NullibleEnumWithoutDefaultLiteral, () => (EnumWithoutDefaultLiteral?)4));
            testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "NullibleEnumWithoutDefaultLiteral", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.NullibleEnumWithoutValues, () => (EnumWithoutValues?)3));
            testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "NullibleEnumWithoutValues", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.CollectionEnum, () => new List<EnumWithDefaultLiteral> { (EnumWithDefaultLiteral)4 }));
            testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "CollectionEnum[0]", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClassWithEnumsCommand>(comp => comp.With(x => x.CollectionStrings, () => default));
            testCommand = fixture.Create<CreateClassWithEnumsCommand>();
            yield return new object[] { testCommand, "CollectionStrings", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            CreateClassWithEnumsCommand testCommand,
            string expectedPropertyName,
            string expectedPhrase)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<System.Guid>();
            // Act
            var act = async () => await validator.Handle(testCommand, () => Task.FromResult(expectedId), CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
                .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
        }

        private ValidationBehaviour<CreateClassWithEnumsCommand, System.Guid> GetValidationBehaviour()
        {
            return new ValidationBehaviour<CreateClassWithEnumsCommand, System.Guid>(new[] { new CreateClassWithEnumsCommandValidator() });
        }
    }
}