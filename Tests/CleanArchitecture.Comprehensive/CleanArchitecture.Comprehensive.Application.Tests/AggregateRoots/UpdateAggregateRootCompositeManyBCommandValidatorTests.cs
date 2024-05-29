using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.UpdateAggregateRootCompositeManyB;
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
    public class UpdateAggregateRootCompositeManyBCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateAggregateRootCompositeManyBCommand testCommand)
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
            fixture.Customize<UpdateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.CompositeAttr, () => default));
            var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, "CompositeAttr", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.Composites, () => default));
            testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
            yield return new object[] { testCommand, "Composites", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            UpdateAggregateRootCompositeManyBCommand testCommand,
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

        private ValidationBehaviour<UpdateAggregateRootCompositeManyBCommand, Unit> GetValidationBehaviour()
        {
            var validatorProvider = Substitute.For<IValidatorProvider>();
            validatorProvider.GetValidator<UpdateAggregateRootCompositeManyBCompositeSingleBBDto>().Returns(c => new UpdateAggregateRootCompositeManyBCompositeSingleBBDtoValidator());
            validatorProvider.GetValidator<UpdateAggregateRootCompositeManyBCompositeManyBBDto>().Returns(c => new UpdateAggregateRootCompositeManyBCompositeManyBBDtoValidator());
            return new ValidationBehaviour<UpdateAggregateRootCompositeManyBCommand, Unit>(new[] { new UpdateAggregateRootCompositeManyBCommandValidator(validatorProvider) });
        }
    }
}