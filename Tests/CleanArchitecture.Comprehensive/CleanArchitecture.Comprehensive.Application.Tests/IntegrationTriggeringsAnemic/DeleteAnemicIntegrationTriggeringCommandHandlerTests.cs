using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Common.Eventing;
using CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.DeleteAnemicIntegrationTriggering;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.ConventionBasedEventPublishing;
using CleanArchitecture.Comprehensive.Domain.Repositories.ConventionBasedEventPublishing;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.IntegrationTriggeringsAnemic
{
    public class DeleteAnemicIntegrationTriggeringCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<IntegrationTriggering>();
            fixture.Customize<DeleteAnemicIntegrationTriggeringCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteAnemicIntegrationTriggeringCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesIntegrationTriggeringFromRepository(
            DeleteAnemicIntegrationTriggeringCommand testCommand,
            IntegrationTriggering existingEntity)
        {
            // Arrange
            var integrationTriggeringRepository = Substitute.For<IIntegrationTriggeringRepository>();
            var eventBus = Substitute.For<IEventBus>();
            integrationTriggeringRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteAnemicIntegrationTriggeringCommandHandler(integrationTriggeringRepository, eventBus);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            integrationTriggeringRepository.Received(1).Remove(Arg.Is<IntegrationTriggering>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidIntegrationTriggeringId_ReturnsNotFound()
        {
            // Arrange
            var integrationTriggeringRepository = Substitute.For<IIntegrationTriggeringRepository>();
            var eventBus = Substitute.For<IEventBus>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteAnemicIntegrationTriggeringCommand>();
            integrationTriggeringRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<IntegrationTriggering>(default));


            var sut = new DeleteAnemicIntegrationTriggeringCommandHandler(integrationTriggeringRepository, eventBus);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}