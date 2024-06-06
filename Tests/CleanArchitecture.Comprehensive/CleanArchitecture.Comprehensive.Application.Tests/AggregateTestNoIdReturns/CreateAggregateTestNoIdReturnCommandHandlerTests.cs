using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateTestNoIdReturns.CreateAggregateTestNoIdReturn;
using CleanArchitecture.Comprehensive.Application.Tests.CRUD.AggregateTestNoIdReturns;
using CleanArchitecture.Comprehensive.Application.Tests.Extensions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateTestNoIdReturns
{
    public class CreateAggregateTestNoIdReturnCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateAggregateTestNoIdReturnCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsAggregateTestNoIdReturnToRepository(CreateAggregateTestNoIdReturnCommand testCommand)
        {
            // Arrange
            var aggregateTestNoIdReturnRepository = Substitute.For<IAggregateTestNoIdReturnRepository>();
            AggregateTestNoIdReturn addedAggregateTestNoIdReturn = null;
            aggregateTestNoIdReturnRepository.OnAdd(ent => addedAggregateTestNoIdReturn = ent);

            var sut = new CreateAggregateTestNoIdReturnCommandHandler(aggregateTestNoIdReturnRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            AggregateTestNoIdReturnAssertions.AssertEquivalent(testCommand, addedAggregateTestNoIdReturn);
        }
    }
}