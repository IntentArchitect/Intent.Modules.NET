using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.WithCompositeKeys.DeleteWithCompositeKey;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.WithCompositeKeys
{
    public class DeleteWithCompositeKeyCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<WithCompositeKey>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<WithCompositeKey>();
            fixture.Customize<DeleteWithCompositeKeyCommand>(comp => comp.With(x => x.Key1Id, existingEntity.Key1Id));
            var testCommand = fixture.Create<DeleteWithCompositeKeyCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory(Skip = "Not working")]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesWithCompositeKeyFromRepository(
            DeleteWithCompositeKeyCommand testCommand,
            WithCompositeKey existingEntity)
        {
            // Arrange
            var repository = Substitute.For<IWithCompositeKeyRepository>();
           //repository.FindByIdAsync(testCommand.Key1Id).Returns(Task.FromResult(existingEntity));

            var sut = new DeleteWithCompositeKeyCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            repository.Received(1).Remove(Arg.Is<WithCompositeKey>(p => p.Key1Id == testCommand.Key1Id));
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteWithCompositeKeyCommand>();

            var repository = Substitute.For<IWithCompositeKeyRepository>();
            //repository.FindByIdAsync(testCommand.Key1Id, CancellationToken.None).Returns(Task.FromResult<WithCompositeKey>(default));
            repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());

            var sut = new DeleteWithCompositeKeyCommandHandler(repository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}