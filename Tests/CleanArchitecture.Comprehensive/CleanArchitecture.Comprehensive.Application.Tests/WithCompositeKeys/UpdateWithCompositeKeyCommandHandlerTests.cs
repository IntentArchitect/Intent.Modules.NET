using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Tests.CompositeKeys.WithCompositeKeys;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.UpdateWithCompositeKey;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.CompositeKeys;
using CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.WithCompositeKeys
{
    public class UpdateWithCompositeKeyCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<WithCompositeKey>();
            fixture.Customize<UpdateWithCompositeKeyCommand>(comp => comp
                .With(x => x.Key1Id, existingEntity.Key1Id)
                .With(x => x.Key2Id, existingEntity.Key2Id));
            var testCommand = fixture.Create<UpdateWithCompositeKeyCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateWithCompositeKeyCommand testCommand,
            WithCompositeKey existingEntity)
        {
            // Arrange
            var withCompositeKeyRepository = Substitute.For<IWithCompositeKeyRepository>();
            withCompositeKeyRepository.FindByIdAsync((testCommand.Key1Id, testCommand.Key2Id), CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new UpdateWithCompositeKeyCommandHandler(withCompositeKeyRepository);

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
            var withCompositeKeyRepository = Substitute.For<IWithCompositeKeyRepository>();
            withCompositeKeyRepository.FindByIdAsync((testCommand.Key1Id, testCommand.Key2Id), CancellationToken.None)!.Returns(Task.FromResult<WithCompositeKey>(default));


            var sut = new UpdateWithCompositeKeyCommandHandler(withCompositeKeyRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}