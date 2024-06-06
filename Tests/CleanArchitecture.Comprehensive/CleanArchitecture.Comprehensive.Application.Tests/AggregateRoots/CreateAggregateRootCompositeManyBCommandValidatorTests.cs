using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.Comprehensive.Application.Common.Behaviours;
using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(CreateAggregateRootCompositeManyBCommand testCommand)
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
            fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.CompositeAttr, () => default));
            var testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, "CompositeAttr", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.Composites, () => default));
            testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, "Composites", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            CreateAggregateRootCompositeManyBCommand testCommand,
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

        private ValidationBehaviour<CreateAggregateRootCompositeManyBCommand, System.Guid> GetValidationBehaviour()
        {
            var validatorProvider = Substitute.For<IValidatorProvider>();
            validatorProvider.GetValidator<CreateAggregateRootCompositeManyBCompositeSingleBBDto>().Returns(c => new CreateAggregateRootCompositeManyBCompositeSingleBBDtoValidator());
            validatorProvider.GetValidator<CreateAggregateRootCompositeManyBCompositeManyBBDto>().Returns(c => new CreateAggregateRootCompositeManyBCompositeManyBBDtoValidator());
            return new ValidationBehaviour<CreateAggregateRootCompositeManyBCommand, System.Guid>(new[] { new CreateAggregateRootCompositeManyBCommandValidator(validatorProvider) });
        }
    }
}