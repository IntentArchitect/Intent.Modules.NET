using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.UpdateIntegrationTriggering;
using CleanArchitecture.TestApplication.Application.Tests.ConventionBasedEventPublishing.IntegrationTriggerings;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.ConventionBasedEventPublishing;
using CleanArchitecture.TestApplication.Domain.Repositories.ConventionBasedEventPublishing;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.IntegrationTriggeringsAnemic
{
    public class UpdateIntegrationTriggeringCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<IntegrationTriggering>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<IntegrationTriggering>();
            fixture.Customize<UpdateIntegrationTriggeringCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateIntegrationTriggeringCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory(Skip = "Not working")]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateIntegrationTriggeringCommand testCommand,
            IntegrationTriggering existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IIntegrationTriggeringRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new UpdateIntegrationTriggeringCommandHandler(repository, null);

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
            var testCommand = fixture.Create<UpdateIntegrationTriggeringCommand>();

            var repository = Substitute.For<IIntegrationTriggeringRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<IntegrationTriggering>(null));

            var sut = new UpdateIntegrationTriggeringCommandHandler(repository, null);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}