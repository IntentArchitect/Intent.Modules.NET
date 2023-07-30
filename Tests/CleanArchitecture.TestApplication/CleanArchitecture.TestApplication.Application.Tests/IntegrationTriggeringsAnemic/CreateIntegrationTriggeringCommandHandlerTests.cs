using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.CreateIntegrationTriggering;
using CleanArchitecture.TestApplication.Application.Tests.ConventionBasedEventPublishing.IntegrationTriggerings;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities.ConventionBasedEventPublishing;
using CleanArchitecture.TestApplication.Domain.Repositories.ConventionBasedEventPublishing;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.IntegrationTriggeringsAnemic
{
    public class CreateIntegrationTriggeringCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateIntegrationTriggeringCommand>() };
        }

        [Theory(Skip = "Not working")]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsIntegrationTriggeringToRepository(CreateIntegrationTriggeringCommand testCommand)
        {
            // Arrange
            var expectedIntegrationTriggeringId = new Fixture().Create<System.Guid>();
            IntegrationTriggering addedIntegrationTriggering = null;
            var repository = Substitute.For<IIntegrationTriggeringRepository>();
            repository.OnAdd(ent => addedIntegrationTriggering = ent);
            repository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedIntegrationTriggering.Id = expectedIntegrationTriggeringId);
            var sut = new CreateIntegrationTriggeringCommandHandler(repository, null);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedIntegrationTriggeringId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            IntegrationTriggeringAssertions.AssertEquivalent(testCommand, addedIntegrationTriggering);
        }
    }
}