using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.UpdateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using FluentAssertions;
using FluentValidation;
using MediatR;
using Xunit;

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots;

public class UpdateAggregateRootCompositeManyBCommandValidatorTests
{
    public static IEnumerable<object[]> GetSuccessfulResultTestData()
    {
        var fixture = new Fixture();
        var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
        testCommand.CompositeAttr = testCommand.CompositeAttr.Substring(0, 20);
        yield return new object[] { testCommand };
    }
    
    [Theory]
    [MemberData(nameof(GetSuccessfulResultTestData))]
    public async Task Validate_WithValidCommand_PassesValidation(UpdateAggregateRootCompositeManyBCommand testCommand)
    {
        // Arrange
        var validator = GetValidationBehaviour();
        var expectedId = Guid.NewGuid();

        // Act
        var result = await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(Unit.Value));

        // Assert
        result.Should().Be(Unit.Value);
    }

    public static IEnumerable<object[]> GetFailedResultTestData()
    {
        var fixture = new Fixture();
        fixture.Customize<UpdateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.CompositeAttr, () => default));
        var testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
        yield return new object[] { testCommand, "CompositeAttr" };
        
        testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
        testCommand.CompositeAttr = "012345678901234567891";
        yield return new object[] { testCommand, "CompositeAttr" };

        fixture = new Fixture();
        fixture.Customize<UpdateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.Composites, () => default));
        testCommand = fixture.Create<UpdateAggregateRootCompositeManyBCommand>();
        yield return new object[] { testCommand, "Composites" };
    }

    [Theory]
    [MemberData(nameof(GetFailedResultTestData))]
    public async Task Validate_WithInvalidCommand_FailsValidation(UpdateAggregateRootCompositeManyBCommand testCommand, string propertyName)
    {
        // Arrange
        var validator = GetValidationBehaviour();

        // Act
        var act = async () => await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(Unit.Value));

        // Assert
        act.Should().ThrowAsync<ValidationException>().Result
            .Which.Errors.Should().Contain(x => x.PropertyName == propertyName);
    }
    
    private ValidationBehaviour<UpdateAggregateRootCompositeManyBCommand, Unit> GetValidationBehaviour()
    {
        return new ValidationBehaviour<UpdateAggregateRootCompositeManyBCommand, Unit>(new[] { new UpdateAggregateRootCompositeManyBCommandValidator() });
    }
}