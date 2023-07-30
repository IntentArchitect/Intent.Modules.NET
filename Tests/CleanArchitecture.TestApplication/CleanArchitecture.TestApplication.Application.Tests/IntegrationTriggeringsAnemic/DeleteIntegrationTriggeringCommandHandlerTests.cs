using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.DeleteIntegrationTriggering;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.ConventionBasedEventPublishing;
using CleanArchitecture.TestApplication.Domain.Repositories.ConventionBasedEventPublishing;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.IntegrationTriggeringsAnemic
{
    public class DeleteIntegrationTriggeringCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<IntegrationTriggering>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<IntegrationTriggering>();
            fixture.Customize<DeleteIntegrationTriggeringCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteIntegrationTriggeringCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory(Skip = "Not working")]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesIntegrationTriggeringFromRepository(
            DeleteIntegrationTriggeringCommand testCommand,
            IntegrationTriggering existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IIntegrationTriggeringRepository>();
            repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(existingEntity));

            var sut = new DeleteIntegrationTriggeringCommandHandler(repository, null);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            repository.Received(1).Remove(Arg.Is<IntegrationTriggering>(p => p.Id == testCommand.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteIntegrationTriggeringCommand>();

            var repository = Substitute.For<IIntegrationTriggeringRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<IntegrationTriggering>(default));
            repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());

            var sut = new DeleteIntegrationTriggeringCommandHandler(repository, null);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}