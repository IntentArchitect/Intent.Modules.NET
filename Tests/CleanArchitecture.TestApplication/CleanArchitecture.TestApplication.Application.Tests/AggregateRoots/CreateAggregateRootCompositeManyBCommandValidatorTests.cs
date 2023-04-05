using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
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

public class CreateAggregateRootCompositeManyBCommandValidatorTests
{
    public static IEnumerable<object[]> GetSuccessfulResultTestData()
    {
        var fixture = new Fixture();
        var testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
        testCommand.CompositeAttr = $"{string.Join(string.Empty, fixture.CreateMany<char>(20))}";
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
        var result = await validator.Handle(testCommand, CancellationToken.None, () => Task.FromResult(expectedId));

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
        fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.CompositeAttr, () => $"{string.Join(string.Empty, fixture.CreateMany<char>(21))}"));
        testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
        yield return new object[] { testCommand, "CompositeAttr", "must be 20 characters or fewer" };

        fixture = new Fixture();
        fixture.Customize<CreateAggregateRootCompositeManyBCommand>(comp => comp.With(x => x.Composites, () => default));
        testCommand = fixture.Create<CreateAggregateRootCompositeManyBCommand>();
        yield return new object[] { testCommand, "Composites", "not be empty" };
    }

    [Theory]
    [MemberData(nameof(GetFailedResultTestData))]
    public async Task Validate_WithInvalidCommand_FailsValidation(CreateAggregateRootCompositeManyBCommand testCommand, string expectedPropertyName, string expectedPhrase)
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

    private ValidationBehaviour<CreateAggregateRootCompositeManyBCommand, System.Guid> GetValidationBehaviour()
    {
        return new ValidationBehaviour<CreateAggregateRootCompositeManyBCommand, System.Guid>(new[] { new CreateAggregateRootCompositeManyBCommandValidator() });
    }
}