using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.CreateSubmission;
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
    public class CreateSubmissionCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateSubmissionCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(CreateSubmissionCommand testCommand)
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
            fixture.Customize<CreateSubmissionCommand>(comp => comp.With(x => x.SubmissionType, () => default));
            var testCommand = fixture.Create<CreateSubmissionCommand>();
            yield return new object[] { testCommand, "SubmissionType", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateSubmissionCommand>(comp => comp.With(x => x.Items, () => default));
            testCommand = fixture.Create<CreateSubmissionCommand>();
            yield return new object[] { testCommand, "Items", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            CreateSubmissionCommand testCommand,
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

        private ValidationBehaviour<CreateSubmissionCommand, System.Guid> GetValidationBehaviour()
        {
            var validatorProvider = Substitute.For<IValidatorProvider>();
            validatorProvider.GetValidator<CreateSubmissionItemDto>().Returns(c => new CreateSubmissionItemDtoValidator());
            return new ValidationBehaviour<CreateSubmissionCommand, System.Guid>(new[] { new CreateSubmissionCommandValidator(validatorProvider) });
        }
    }
}