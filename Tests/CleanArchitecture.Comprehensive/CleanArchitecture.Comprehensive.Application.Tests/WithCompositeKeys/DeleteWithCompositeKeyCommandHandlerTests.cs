using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.DeleteWithCompositeKey;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CompositeKeys;
using CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.WithCompositeKeys
{
    public class DeleteWithCompositeKeyCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<WithCompositeKey>();
            fixture.Customize<DeleteWithCompositeKeyCommand>(comp => comp
                .With(x => x.Key1Id, existingEntity.Key1Id)
                .With(x => x.Key2Id, existingEntity.Key2Id));
            var testCommand = fixture.Create<DeleteWithCompositeKeyCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesWithCompositeKeyFromRepository(
            DeleteWithCompositeKeyCommand testCommand,
            WithCompositeKey existingEntity)
        {
            // Arrange
            var withCompositeKeyRepository = Substitute.For<IWithCompositeKeyRepository>();
            withCompositeKeyRepository.FindByIdAsync((testCommand.Key1Id, testCommand.Key2Id), CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteWithCompositeKeyCommandHandler(withCompositeKeyRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            withCompositeKeyRepository.Received(1).Remove(Arg.Is<WithCompositeKey>(p => testCommand.Key1Id == p.Key1Id && testCommand.Key2Id == p.Key2Id));
        }

        [Fact]
        public async Task Handle_WithInvalidWithCompositeKeyId_ReturnsNotFound()
        {
            // Arrange
            var withCompositeKeyRepository = Substitute.For<IWithCompositeKeyRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteWithCompositeKeyCommand>();
            withCompositeKeyRepository.FindByIdAsync((testCommand.Key1Id, testCommand.Key2Id), CancellationToken.None)!.Returns(Task.FromResult<WithCompositeKey>(default));


            var sut = new DeleteWithCompositeKeyCommandHandler(withCompositeKeyRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}