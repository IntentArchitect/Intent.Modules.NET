using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.ClassContainers.UpdateClassContainer;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.ClassContainers
{
    public class UpdateClassContainerCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<ClassContainer>();
            fixture.Customize<UpdateClassContainerCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateClassContainerCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateClassContainerCommand testCommand,
            ClassContainer existingEntity)
        {
            // Arrange
            var classContainerRepository = Substitute.For<IClassContainerRepository>();
            classContainerRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new UpdateClassContainerCommandHandler(classContainerRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            ClassContainerAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateClassContainerCommand>();
            var classContainerRepository = Substitute.For<IClassContainerRepository>();
            classContainerRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<ClassContainer>(default));


            var sut = new UpdateClassContainerCommandHandler(classContainerRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}