using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.Common.Eventing;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.CreateAnemicIntegrationTriggering;
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
    public class CreateAnemicIntegrationTriggeringCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateAnemicIntegrationTriggeringCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsIntegrationTriggeringToRepository(CreateAnemicIntegrationTriggeringCommand testCommand)
        {
            // Arrange
            var integrationTriggeringRepository = Substitute.For<IIntegrationTriggeringRepository>();
            var eventBus = Substitute.For<IEventBus>();
            var expectedIntegrationTriggeringId = new Fixture().Create<System.Guid>();
            IntegrationTriggering addedIntegrationTriggering = null;
            integrationTriggeringRepository.OnAdd(ent => addedIntegrationTriggering = ent);
            integrationTriggeringRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedIntegrationTriggering.Id = expectedIntegrationTriggeringId);

            var sut = new CreateAnemicIntegrationTriggeringCommandHandler(integrationTriggeringRepository, eventBus);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedIntegrationTriggeringId);
            await integrationTriggeringRepository.UnitOfWork.Received(1).SaveChangesAsync();
            IntegrationTriggeringAssertions.AssertEquivalent(testCommand, addedIntegrationTriggering);
        }
    }
}