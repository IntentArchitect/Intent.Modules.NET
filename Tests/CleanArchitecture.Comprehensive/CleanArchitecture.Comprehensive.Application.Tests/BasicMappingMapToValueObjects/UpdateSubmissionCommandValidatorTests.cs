using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.UpdateSubmission;
using CleanArchitecture.Comprehensive.Application.Common.Behaviours;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.BasicMappingMapToValueObjects
{
    public class UpdateSubmissionCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateSubmissionCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateSubmissionCommand testCommand)
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
            fixture.Customize<UpdateSubmissionCommand>(comp => comp.With(x => x.SubmissionType, () => default));
            var testCommand = fixture.Create<UpdateSubmissionCommand>();
            yield return new object[] { testCommand, "SubmissionType", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateSubmissionCommand>(comp => comp.With(x => x.Items, () => default));
            testCommand = fixture.Create<UpdateSubmissionCommand>();
            yield return new object[] { testCommand, "Items", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            UpdateSubmissionCommand testCommand,
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

        private ValidationBehaviour<UpdateSubmissionCommand, Unit> GetValidationBehaviour()
        {
            var validatorProvider = Substitute.For<IValidatorProvider>();
            validatorProvider.GetValidator<UpdateSubmissionItemDto>().Returns(c => new UpdateSubmissionItemDtoValidator());
            return new ValidationBehaviour<UpdateSubmissionCommand, Unit>(new[] { new UpdateSubmissionCommandValidator(validatorProvider) });
        }
    }
}