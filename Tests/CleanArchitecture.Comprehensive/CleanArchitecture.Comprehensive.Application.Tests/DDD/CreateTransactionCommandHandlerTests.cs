using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.DDD;
using CleanArchitecture.Comprehensive.Application.DDD.CreateTransaction;
using CleanArchitecture.Comprehensive.Application.Tests.DDD.Transactions;
using CleanArchitecture.Comprehensive.Application.Tests.Extensions;
using CleanArchitecture.Comprehensive.Domain.DDD;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.DDD
{
    public class CreateTransactionCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateTransactionCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsTransactionToRepository(CreateTransactionCommand testCommand)
        {
            // Arrange
            var transactionRepository = Substitute.For<ITransactionRepository>();
            Transaction addedTransaction = null;
            transactionRepository.OnAdd(ent => addedTransaction = ent);

            var sut = new CreateTransactionCommandHandler(transactionRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            TransactionAssertions.AssertEquivalent(testCommand, addedTransaction);
        }
    }
}