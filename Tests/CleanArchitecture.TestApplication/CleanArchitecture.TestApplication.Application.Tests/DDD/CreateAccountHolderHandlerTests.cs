using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.DDD.CreateAccountHolder;
using CleanArchitecture.TestApplication.Application.Tests.DDD.AccountHolders;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities.DDD;
using CleanArchitecture.TestApplication.Domain.Repositories.DDD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.DDD
{
    public class CreateAccountHolderHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateAccountHolder>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsAccountHolderToRepository(CreateAccountHolder testCommand)
        {
            // Arrange
            var accountHolderRepository = Substitute.For<IAccountHolderRepository>();
            var expectedAccountHolderId = new Fixture().Create<System.Guid>();
            AccountHolder addedAccountHolder = null;
            accountHolderRepository.OnAdd(ent => addedAccountHolder = ent);
            accountHolderRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedAccountHolder.Id = expectedAccountHolderId);

            var sut = new CreateAccountHolderHandler(accountHolderRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedAccountHolderId);
            await accountHolderRepository.UnitOfWork.Received(1).SaveChangesAsync();
            AccountHolderAssertions.AssertEquivalent(testCommand, addedAccountHolder);
        }
    }
}