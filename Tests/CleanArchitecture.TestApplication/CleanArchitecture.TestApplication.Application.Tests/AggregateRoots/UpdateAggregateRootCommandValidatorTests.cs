using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRoot;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class UpdateAggregateRootCommandValidatorTests
{
    public static IEnumerable<object[]> GetSuccessfulResultTestData()
    {
        var fixture = new Fixture();
        var testCommand = fixture.Create<UpdateAggregateRootCommand>();
        testCommand.AggregateAttr = $"{string.Join(string.Empty, fixture.CreateMany<char>(20))}";
        yield return new object[] { testCommand };
    }

    [Theory]
    [MemberData(nameof(GetSuccessfulResultTestData))]
    public async Task Validate_WithValidCommand_PassesValidation(UpdateAggregateRootCommand testCommand)
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
        fixture.Customize<UpdateAggregateRootCommand>(comp => comp.With(x => x.AggregateAttr, () => default));
        var testCommand = fixture.Create<UpdateAggregateRootCommand>();
        yield return new object[] { testCommand, "AggregateAttr", "not be empty" };

        fixture = new Fixture();
        fixture.Customize<UpdateAggregateRootCommand>(comp => comp.With(x => x.AggregateAttr, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(21))}"));
        testCommand = fixture.Create<UpdateAggregateRootCommand>();
        yield return new object[] { testCommand, "AggregateAttr", "must be 20 characters or fewer" };

        fixture = new Fixture();
        fixture.Customize<UpdateAggregateRootCommand>(comp => comp.With(x => x.Composites, () => default));
        testCommand = fixture.Create<UpdateAggregateRootCommand>();
        yield return new object[] { testCommand, "Composites", "not be empty" };
    }

    [Theory]
    [MemberData(nameof(GetFailedResultTestData))]
    public async Task Validate_WithInvalidCommand_FailsValidation(UpdateAggregateRootCommand testCommand, string expectedPropertyName, string expectedPhrase)
    {
        // Arrange
        var validator = GetValidationBehaviour();
        // Act
        var act = async () => await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(Unit.Value));

        // Assert
        act.Should().ThrowAsync<ValidationException>().Result
        .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
    }

    private ValidationBehaviour<UpdateAggregateRootCommand, Unit> GetValidationBehaviour()
    {
        return new ValidationBehaviour<UpdateAggregateRootCommand, Unit>(new[] { new UpdateAggregateRootCommandValidator() });
    }
}