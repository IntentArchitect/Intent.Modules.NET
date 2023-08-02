using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.WithCompositeKeys.UpdateWithCompositeKey;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.WithCompositeKeys
{
    public class UpdateWithCompositeKeyCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<WithCompositeKey>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<WithCompositeKey>();
            fixture.Customize<UpdateWithCompositeKeyCommand>(comp => comp.With(x => x.Key1Id, existingEntity.Key1Id));
            var testCommand = fixture.Create<UpdateWithCompositeKeyCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory(Skip = "Not working")]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateWithCompositeKeyCommand testCommand,
            WithCompositeKey existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IWithCompositeKeyRepository>();
            //repository.FindByIdAsync(testCommand.Key1Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new UpdateWithCompositeKeyCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            WithCompositeKeyAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateWithCompositeKeyCommand>();

            var repository = Substitute.For<IWithCompositeKeyRepository>();
            //repository.FindByIdAsync(testCommand.Key1Id, CancellationToken.None).Returns(Task.FromResult<WithCompositeKey>(null));

            var sut = new UpdateWithCompositeKeyCommandHandler(repository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}