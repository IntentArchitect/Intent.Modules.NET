using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
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

public class CreateAggregateRootCommandValidatorTests
{
    public static IEnumerable<object[]> GetSuccessfulResultTestData()
    {
        var fixture = new Fixture();
        var testCommand = fixture.Create<CreateAggregateRootCommand>();
        testCommand.AggregateAttr = "01234567890123456789";
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
        var result = await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(expectedId));

        // Assert
        result.Should().Be(expectedId);
    }

    public static IEnumerable<object[]> GetFailedResultTestData()
    {
        var fixture = new Fixture();
        fixture.Customize<CreateAggregateRootCommand>(comp => comp.With(x => x.AggregateAttr, () => default));
        var testCommand = fixture.Create<CreateAggregateRootCommand>();
        yield return new object[] { testCommand, "AggregateAttr", "not be empty" };

        fixture = new Fixture();
        fixture.Customize<CreateAggregateRootCommand>(comp => comp.With(x => x.AggregateAttr, () => "012345678901234567890"));
        testCommand = fixture.Create<CreateAggregateRootCommand>();
        yield return new object[] { testCommand, "AggregateAttr", "must be 20 characters or fewer" };

        fixture = new Fixture();
        fixture.Customize<CreateAggregateRootCommand>(comp => comp.With(x => x.Composites, () => default));
        testCommand = fixture.Create<CreateAggregateRootCommand>();
        yield return new object[] { testCommand, "Composites", "not be empty" };
    }

    [Theory]
    [MemberData(nameof(GetFailedResultTestData))]
    public async Task Validate_WithInvalidCommand_FailsValidation(CreateAggregateRootCommand testCommand, string expectedPropertyName, string expectedPhrase)
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

    private ValidationBehaviour<CreateAggregateRootCommand, System.Guid> GetValidationBehaviour()
    {
        return new ValidationBehaviour<CreateAggregateRootCommand, System.Guid>(new[] { new CreateAggregateRootCommandValidator() });
    }
}