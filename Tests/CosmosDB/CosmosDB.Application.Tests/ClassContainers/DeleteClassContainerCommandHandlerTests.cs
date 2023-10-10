using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.ClassContainers.DeleteClassContainer;
using CosmosDB.Domain.Common;
using CosmosDB.Domain.Common.Exceptions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.ClassContainers
{
    public class DeleteClassContainerCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<ClassContainer>();
            fixture.Customize<DeleteClassContainerCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteClassContainerCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesClassContainerFromRepository(
            DeleteClassContainerCommand testCommand,
            ClassContainer existingEntity)
        {
            // Arrange
            var classContainerRepository = Substitute.For<IClassContainerRepository>();
            classContainerRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteClassContainerCommandHandler(classContainerRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            classContainerRepository.Received(1).Remove(Arg.Is<ClassContainer>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidClassContainerId_ReturnsNotFound()
        {
            // Arrange
            var classContainerRepository = Substitute.For<IClassContainerRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteClassContainerCommand>();
            classContainerRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<ClassContainer>(default));


            var sut = new DeleteClassContainerCommandHandler(classContainerRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}