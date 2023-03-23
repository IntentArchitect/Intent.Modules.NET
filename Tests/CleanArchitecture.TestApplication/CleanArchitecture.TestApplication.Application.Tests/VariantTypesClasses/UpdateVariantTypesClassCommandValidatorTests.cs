using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using CleanArchitecture.TestApplication.Application.VariantTypesClasses.UpdateVariantTypesClass;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.VariantTypesClasses
{
    public class UpdateVariantTypesClassCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateVariantTypesClassCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateVariantTypesClassCommand testCommand)
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
            fixture.Customize<UpdateVariantTypesClassCommand>(comp => comp.With(x => x.StrCollection, () => default));
            var testCommand = fixture.Create<UpdateVariantTypesClassCommand>();
            yield return new object[] { testCommand, "StrCollection", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateVariantTypesClassCommand>(comp => comp.With(x => x.IntCollection, () => default));
            testCommand = fixture.Create<UpdateVariantTypesClassCommand>();
            yield return new object[] { testCommand, "IntCollection", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(UpdateVariantTypesClassCommand testCommand, string expectedPropertyName, string expectedPhrase)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            // Act
            var act = async () => await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(Unit.Value));

            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
            .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
        }

        private ValidationBehaviour<UpdateVariantTypesClassCommand, Unit> GetValidationBehaviour()
        {
            return new ValidationBehaviour<UpdateVariantTypesClassCommand, Unit>(new[] { new UpdateVariantTypesClassCommandValidator() });
        }
    }
}