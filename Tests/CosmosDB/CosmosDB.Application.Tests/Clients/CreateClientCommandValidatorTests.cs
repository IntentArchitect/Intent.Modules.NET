using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.Clients.CreateClient;
using CosmosDB.Application.Common.Behaviours;
using CosmosDB.Domain;
using FluentAssertions;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.FluentValidation.FluentValidationTest", Version = "1.0")]

namespace CosmosDB.Application.Tests.Clients
{
    public class CreateClientCommandValidatorTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateClientCommand>();
            yield return new object[] { testCommand };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Validate_WithValidCommand_PassesValidation(CreateClientCommand testCommand)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<string>();
            // Act
            var result = await validator.Handle(testCommand, () => Task.FromResult(expectedId), CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
        }

        public static IEnumerable<object[]> GetFailedResultTestData()
        {
            var fixture = new Fixture();
            fixture.Customize<CreateClientCommand>(comp => comp.With(x => x.Type, () => (ClientType)2));
            var testCommand = fixture.Create<CreateClientCommand>();
            yield return new object[] { testCommand, "Type", "has a range of values which does not include" };

            fixture = new Fixture();
            fixture.Customize<CreateClientCommand>(comp => comp.With(x => x.Name, () => default));
            testCommand = fixture.Create<CreateClientCommand>();
            yield return new object[] { testCommand, "Name", "not be empty" };
        }

        [Theory]
        [MemberData(nameof(GetFailedResultTestData))]
        public async Task Validate_WithInvalidCommand_FailsValidation(
            CreateClientCommand testCommand,
            string expectedPropertyName,
            string expectedPhrase)
        {
            // Arrange
            var validator = GetValidationBehaviour();
            var expectedId = new Fixture().Create<string>();
            // Act
            var act = async () => await validator.Handle(testCommand, () => Task.FromResult(expectedId), CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<ValidationException>().Result
            .Which.Errors.Should().Contain(x => x.PropertyName == expectedPropertyName && x.ErrorMessage.Contains(expectedPhrase));
        }

        private ValidationBehaviour<CreateClientCommand, string> GetValidationBehaviour()
        {
            return new ValidationBehaviour<CreateClientCommand, string>(new[] { new CreateClientCommandValidator() });
        }
    }
}