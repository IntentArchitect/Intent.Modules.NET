using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Common.Behaviours;
using CleanArchitecture.Comprehensive.Application.UniqueIndexConstraint.ClassicMapping.UpdateAggregateWithUniqueConstraintIndexStereotype;
using CleanArchitecture.Comprehensive.Domain.Repositories.UniqueIndexConstraint;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.UniqueIndexConstraint.ClassicMapping
{
    public class UpdateAggregateWithUniqueConstraintIndexStereotypeCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>();
            testCommand.SingleUniqueField = $"{string.Join(string.Empty, fixture.CreateMany<char>(256))}";
            testCommand.CompUniqueFieldA = $"{string.Join(string.Empty, fixture.CreateMany<char>(256))}";
            testCommand.CompUniqueFieldB = $"{string.Join(string.Empty, fixture.CreateMany<char>(256))}";
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(UpdateAggregateWithUniqueConstraintIndexStereotypeCommand testCommand)
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
            fixture.Customize<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>(comp => comp
                .With(x => x.SingleUniqueField, () => default)
                .With(x => x.CompUniqueFieldA, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}")
                .With(x => x.CompUniqueFieldB, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}"));
            var testCommand = fixture.Create<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>();
            yield return new object[] { testCommand, "SingleUniqueField", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>(comp => comp
                .With(x => x.SingleUniqueField, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(257))}")
                .With(x => x.CompUniqueFieldA, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}")
                .With(x => x.CompUniqueFieldB, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}"));
            testCommand = fixture.Create<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>();
            yield return new object[] { testCommand, "SingleUniqueField", "must be 256 characters or fewer" };

            fixture = new Fixture();
            fixture.Customize<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>(comp => comp
                .With(x => x.CompUniqueFieldA, () => default)
                .With(x => x.SingleUniqueField, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}")
                .With(x => x.CompUniqueFieldB, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}"));
            testCommand = fixture.Create<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>();
            yield return new object[] { testCommand, "CompUniqueFieldA", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>(comp => comp
                .With(x => x.CompUniqueFieldA, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(257))}")
                .With(x => x.SingleUniqueField, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}")
                .With(x => x.CompUniqueFieldB, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}"));
            testCommand = fixture.Create<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>();
            yield return new object[] { testCommand, "CompUniqueFieldA", "must be 256 characters or fewer" };

            fixture = new Fixture();
            fixture.Customize<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>(comp => comp
                .With(x => x.CompUniqueFieldB, () => default)
                .With(x => x.SingleUniqueField, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}")
                .With(x => x.CompUniqueFieldA, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}"));
            testCommand = fixture.Create<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>();
            yield return new object[] { testCommand, "CompUniqueFieldB", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>(comp => comp
                .With(x => x.CompUniqueFieldB, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(257))}")
                .With(x => x.SingleUniqueField, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}")
                .With(x => x.CompUniqueFieldA, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(255))}"));
            testCommand = fixture.Create<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>();
            yield return new object[] { testCommand, "CompUniqueFieldB", "must be 256 characters or fewer" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            UpdateAggregateWithUniqueConstraintIndexStereotypeCommand testCommand,
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

        private ValidationBehaviour<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand, Unit> GetValidationBehaviour()
        {
            var aggregateWithUniqueConstraintIndexStereotypeRepository = Substitute.For<IAggregateWithUniqueConstraintIndexStereotypeRepository>();
            return new ValidationBehaviour<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand, Unit>(new[] { new UpdateAggregateWithUniqueConstraintIndexStereotypeCommandValidator(aggregateWithUniqueConstraintIndexStereotypeRepository) });
        }
    }
}