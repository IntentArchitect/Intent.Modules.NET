using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRoot;
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
    public class CreateAggregateRootCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateAggregateRootCommand>();
            testCommand.LimitedDomain = $"{string.Join(string.Empty, fixture.CreateMany<char>(10))}";
            testCommand.LimitedService = $"{string.Join(string.Empty, fixture.CreateMany<char>(20))}";
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(CreateAggregateRootCommand testCommand)
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
            fixture.Customize<CreateAggregateRootCommand>(comp => comp
                .With(x => x.AggregateAttr, () => default)
                .With(x => x.LimitedDomain, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(9))}")
                .With(x => x.LimitedService, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(19))}"));
            var testCommand = fixture.Create<CreateAggregateRootCommand>();
            yield return new object[] { testCommand, "AggregateAttr", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateAggregateRootCommand>(comp => comp
                .With(x => x.Composites, () => default)
                .With(x => x.LimitedDomain, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(9))}")
                .With(x => x.LimitedService, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(19))}"));
            testCommand = fixture.Create<CreateAggregateRootCommand>();
            yield return new object[] { testCommand, "Composites", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateAggregateRootCommand>(comp => comp
                .With(x => x.LimitedDomain, () => default)
                .With(x => x.LimitedService, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(19))}"));
            testCommand = fixture.Create<CreateAggregateRootCommand>();
            yield return new object[] { testCommand, "LimitedDomain", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateAggregateRootCommand>(comp => comp
                .With(x => x.LimitedDomain, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(11))}")
                .With(x => x.LimitedService, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(19))}"));
            testCommand = fixture.Create<CreateAggregateRootCommand>();
            yield return new object[] { testCommand, "LimitedDomain", "must be 10 characters or fewer" };

            fixture = new Fixture();
            fixture.Customize<CreateAggregateRootCommand>(comp => comp
                .With(x => x.LimitedService, () => default)
                .With(x => x.LimitedDomain, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(9))}"));
            testCommand = fixture.Create<CreateAggregateRootCommand>();
            yield return new object[] { testCommand, "LimitedService", "not be empty" };

            fixture = new Fixture();
            fixture.Customize<CreateAggregateRootCommand>(comp => comp
                .With(x => x.LimitedService, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(21))}")
                .With(x => x.LimitedDomain, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(9))}"));
            testCommand = fixture.Create<CreateAggregateRootCommand>();
            yield return new object[] { testCommand, "LimitedService", "must be 20 characters or fewer" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            CreateAggregateRootCommand testCommand,
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

        private ValidationBehaviour<CreateAggregateRootCommand, System.Guid> GetValidationBehaviour()
        {
            var validatorProvider = Substitute.For<IValidatorProvider>();
            validatorProvider.GetValidator<CreateAggregateRootCompositeManyBDto>().Returns(c => new CreateAggregateRootCompositeManyBDtoValidator(validatorProvider));
            validatorProvider.GetValidator<CreateAggregateRootCompositeManyBCompositeSingleBBDto>().Returns(c => new CreateAggregateRootCompositeManyBCompositeSingleBBDtoValidator());
            validatorProvider.GetValidator<CreateAggregateRootCompositeManyBCompositeManyBBDto>().Returns(c => new CreateAggregateRootCompositeManyBCompositeManyBBDtoValidator());
            validatorProvider.GetValidator<CreateAggregateRootCompositeSingleADto>().Returns(c => new CreateAggregateRootCompositeSingleADtoValidator(validatorProvider));
            validatorProvider.GetValidator<CreateAggregateRootCompositeSingleACompositeSingleAADto>().Returns(c => new CreateAggregateRootCompositeSingleACompositeSingleAADtoValidator());
            validatorProvider.GetValidator<CreateAggregateRootCompositeSingleACompositeManyAADto>().Returns(c => new CreateAggregateRootCompositeSingleACompositeManyAADtoValidator());
            validatorProvider.GetValidator<CreateAggregateRootAggregateSingleCDto>().Returns(c => new CreateAggregateRootAggregateSingleCDtoValidator());
            return new ValidationBehaviour<CreateAggregateRootCommand, System.Guid>(new[] { new CreateAggregateRootCommandValidator(validatorProvider) });
        }
    }
}