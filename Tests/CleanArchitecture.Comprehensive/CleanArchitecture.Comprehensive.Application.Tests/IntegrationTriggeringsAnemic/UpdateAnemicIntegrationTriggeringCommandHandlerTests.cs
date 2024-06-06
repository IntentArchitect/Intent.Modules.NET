using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Common.Eventing;
using CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.UpdateAnemicIntegrationTriggering;
using CleanArchitecture.Comprehensive.Application.Tests.ConventionBasedEventPublishing.IntegrationTriggerings;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.ConventionBasedEventPublishing;
using CleanArchitecture.Comprehensive.Domain.Repositories.ConventionBasedEventPublishing;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.IntegrationTriggeringsAnemic
{
    public class UpdateAnemicIntegrationTriggeringCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<IntegrationTriggering>();
            fixture.Customize<UpdateAnemicIntegrationTriggeringCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateAnemicIntegrationTriggeringCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateAnemicIntegrationTriggeringCommand testCommand,
            IntegrationTriggering existingEntity)
        {
            // Arrange
            var integrationTriggeringRepository = Substitute.For<IIntegrationTriggeringRepository>();
            var eventBus = Substitute.For<IEventBus>();
            integrationTriggeringRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new UpdateAnemicIntegrationTriggeringCommandHandler(integrationTriggeringRepository, eventBus);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            IntegrationTriggeringAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateAnemicIntegrationTriggeringCommand>();
            var integrationTriggeringRepository = Substitute.For<IIntegrationTriggeringRepository>();
            var eventBus = Substitute.For<IEventBus>();
            integrationTriggeringRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<IntegrationTriggering>(default));


            var sut = new UpdateAnemicIntegrationTriggeringCommandHandler(integrationTriggeringRepository, eventBus);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}