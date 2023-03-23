using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ImplicitKeyAggrRoots
{
    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<System.Guid>();
            // Act
            var result = await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(expectedId));

            // Assert
            result.Should().Be(expectedId);
        }

        public static IEnumerable<object[]> GetFailedResultTestData()
        {
            var fixture = new Fixture();
            fixture.Customize<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>(comp => comp.With(x => x.Attribute, () => default));
            var testCommand = fixture.Create<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand>();
            yield return new object[] { testCommand, "Attribute", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand testCommand, string expectedPropertyName, string expectedPhrase)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<System.Guid>();
            // Act
            var act = async () => await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(expectedId));

            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
            .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
        }

        private ValidationBehaviour<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand, System.Guid> GetValidationBehaviour()
        {
            return new ValidationBehaviour<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand, System.Guid>(new[] { new CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommandValidator() });
        }
    }
}