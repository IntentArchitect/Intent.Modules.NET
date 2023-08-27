using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AccountHolders.UpdateNoteAccountHolder;
using CleanArchitecture.TestApplication.Application.Tests.DDD.AccountHolders;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using CleanArchitecture.TestApplication.Domain.Repositories.DDD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedUpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AccountHolders
{
    public class UpdateNoteAccountHolderCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingOwnerEntity = fixture.Create<AccountHolder>();
            var existingEntity = existingOwnerEntity.Accounts.First();
            existingEntity.AccountHolderId = existingOwnerEntity.Id;
            fixture.Customize<UpdateNoteAccountHolderCommand>(comp => comp
                .With(x => x.AccountHolderId, existingOwnerEntity.Id)
                .With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateNoteAccountHolderCommand>();
            yield return new object[] { testCommand, existingOwnerEntity, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesAccount(
            UpdateNoteAccountHolderCommand testCommand,
            AccountHolder existingOwnerEntity,
            Account existingEntity)
        {
            // Arrange
            var accountHolderRepository = Substitute.For<IAccountHolderRepository>();
            accountHolderRepository.FindByIdAsync(testCommand.AccountHolderId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new UpdateNoteAccountHolderCommandHandler(accountHolderRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            AccountHolderAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidAccountHolderId_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateNoteAccountHolderCommand>();
            var accountHolderRepository = Substitute.For<IAccountHolderRepository>();
            accountHolderRepository.FindByIdAsync(testCommand.AccountHolderId, CancellationToken.None)!.Returns(Task.FromResult<AccountHolder>(default));

            var sut = new UpdateNoteAccountHolderCommandHandler(accountHolderRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WithInvalidAccountId_ReturnsNotFound()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            fixture.Customize<AccountHolder>(comp => comp.With(p => p.Accounts, new List<Account>()));
            var existingOwnerEntity = fixture.Create<AccountHolder>();
            fixture.Customize<UpdateNoteAccountHolderCommand>(comp => comp
                .With(p => p.AccountHolderId, existingOwnerEntity.Id));
            var testCommand = fixture.Create<UpdateNoteAccountHolderCommand>();
            var accountHolderRepository = Substitute.For<IAccountHolderRepository>();
            accountHolderRepository.FindByIdAsync(testCommand.AccountHolderId, CancellationToken.None)!.Returns(Task.FromResult(existingOwnerEntity));

            var sut = new UpdateNoteAccountHolderCommandHandler(accountHolderRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}