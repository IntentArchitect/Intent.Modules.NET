using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Xunit;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class CreateAggregateRootCompositeManyBCommandValidatorTests
{
    public static IEnumerable<object[]> GetSuccessfulResultTestData()
    {
        var fixture = new Fixture();
        var testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
        testCommand.CompositeAttr = testCommand.CompositeAttr.Substring(0, 20);
        yield return new object[] { testCommand };
    }
    
    [Theory]
    [MemberData(nameof(GetSuccessfulResultTestData))]
    public async Task Validate_WithValidCommand_PassesValidation(CreateAggregateRootCompositeManyBCommand testCommand)
    {
        // Arrange
        var validator = GetValidationBehaviour();
        var expectedId = Guid.NewGuid();

        // Act
        var result = await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(expectedId));

        // Assert
        result.Should().Be(expectedId);
    }

    public static IEnumerable<object[]> GetFailedResultTestData()
    {
        var fixture = new Fixture();
        fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.CompositeAttr, () => default));
        var testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
        yield return new object[] { testCommand, "CompositeAttr" };
        
        testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
        testCommand.CompositeAttr = "012345678901234567891";
        yield return new object[] { testCommand, "CompositeAttr" };

        fixture = new Fixture();
        fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.Composites, () => default));
        testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
        yield return new object[] { testCommand, "Composites" };
    }

    [Theory]
    [MemberData(nameof(GetFailedResultTestData))]
    public async Task Validate_WithInvalidCommand_FailsValidation(CreateAggregateRootCompositeManyBCommand testCommand, string propertyName)
    {
        // Arrange
        var validator = GetValidationBehaviour();
        var expectedId = Guid.NewGuid();

        // Act
        var act = async () => await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(expectedId));

        // Assert
        act.Should().ThrowAsync<ValidationException>().Result
            .Which.Errors.Should().Contain(x => x.PropertyName == propertyName);
    }
    
    private ValidationBehaviour<CreateAggregateRootCompositeManyBCommand, Guid> GetValidationBehaviour()
    {
        return new ValidationBehaviour<CreateAggregateRootCompositeManyBCommand, Guid>(new[] { new CreateAggregateRootCompositeManyBCommandValidator() });
    }
}