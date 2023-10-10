using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.ClassContainers.CreateClassContainer;
using CosmosDB.Application.Tests.Extensions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.ClassContainers
{
    public class CreateClassContainerCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateClassContainerCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsClassContainerToRepository(CreateClassContainerCommand testCommand)
        {
            // Arrange
            var classContainerRepository = Substitute.For<IClassContainerRepository>();
            var expectedClassContainerId = new Fixture().Create<string>();
            ClassContainer addedClassContainer = null;
            classContainerRepository.OnAdd(ent => addedClassContainer = ent);
            classContainerRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedClassContainer.Id = expectedClassContainerId);

            var sut = new CreateClassContainerCommandHandler(classContainerRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedClassContainerId);
            await classContainerRepository.UnitOfWork.Received(1).SaveChangesAsync();
            ClassContainerAssertions.AssertEquivalent(testCommand, addedClassContainer);
        }
    }
}