using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Common.Behaviours;
using CosmosDB.Application.DerivedOfTS.UpdateDerivedOfT;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CosmosDB.Application.Tests.DerivedOfTS
{
    public class UpdateDerivedOfTCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateDerivedOfTCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateDerivedOfTCommand testCommand)
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
            fixture.Customize<UpdateDerivedOfTCommand>(comp => comp.With(x => x.Id, () => default));
            var testCommand = fixture.Create<UpdateDerivedOfTCommand>();
            yield return new object[] { testCommand, "Id", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateDerivedOfTCommand>(comp => comp.With(x => x.DerivedAttribute, () => default));
            testCommand = fixture.Create<UpdateDerivedOfTCommand>();
            yield return new object[] { testCommand, "DerivedAttribute", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            UpdateDerivedOfTCommand testCommand,
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

        private ValidationBehaviour<UpdateDerivedOfTCommand, Unit> GetValidationBehaviour()
        {
            return new ValidationBehaviour<UpdateDerivedOfTCommand, Unit>(new[] { new UpdateDerivedOfTCommandValidator() });
        }
    }
}