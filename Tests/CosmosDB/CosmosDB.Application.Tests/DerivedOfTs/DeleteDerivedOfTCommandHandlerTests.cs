using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.DerivedOfTS.DeleteDerivedOfT;
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

namespace CosmosDB.Application.Tests.DerivedOfTS
{
    public class DeleteDerivedOfTCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<DerivedOfT>();
            fixture.Customize<DeleteDerivedOfTCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteDerivedOfTCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesDerivedOfTFromRepository(
            DeleteDerivedOfTCommand testCommand,
            DerivedOfT existingEntity)
        {
            // Arrange
            var derivedOfTRepository = Substitute.For<IDerivedOfTRepository>();
            derivedOfTRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteDerivedOfTCommandHandler(derivedOfTRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            derivedOfTRepository.Received(1).Remove(Arg.Is<DerivedOfT>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidDerivedOfTId_ReturnsNotFound()
        {
            // Arrange
            var derivedOfTRepository = Substitute.For<IDerivedOfTRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteDerivedOfTCommand>();
            derivedOfTRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<DerivedOfT>(default));


            var sut = new DeleteDerivedOfTCommandHandler(derivedOfTRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}