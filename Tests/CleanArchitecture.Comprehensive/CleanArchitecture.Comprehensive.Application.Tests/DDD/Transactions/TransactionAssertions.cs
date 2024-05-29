using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.DDD.CreateTransaction;
using CleanArchitecture.Comprehensive.Domain.Entities.DDD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.DDD.Transactions
{
    public static class TransactionAssertions
    {
        public static void AssertEquivalent(CreateTransactionCommand expectedDto, Transaction actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Current.Should().BeEquivalentTo(expectedDto.Current);
            actualEntity.Description.Should().Be(expectedDto.Description);
            actualEntity.AccountId.Should().Be(expectedDto.AccountId);
#warning Field not a composite association: Account
        }
    }
}