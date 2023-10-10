using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CosmosDB.Application.DerivedOfTS.CreateDerivedOfT;
using CosmosDB.Application.Tests.DerivedOfTs;
using CosmosDB.Application.Tests.Extensions;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CosmosDB.Application.Tests.DerivedOfTS
{
    public class CreateDerivedOfTCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateDerivedOfTCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsDerivedOfTToRepository(CreateDerivedOfTCommand testCommand)
        {
            // Arrange
            var derivedOfTRepository = Substitute.For<IDerivedOfTRepository>();
            var expectedDerivedOfTId = new Fixture().Create<string>();
            DerivedOfT addedDerivedOfT = null;
            derivedOfTRepository.OnAdd(ent => addedDerivedOfT = ent);
            derivedOfTRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedDerivedOfT.Id = expectedDerivedOfTId);

            var sut = new CreateDerivedOfTCommandHandler(derivedOfTRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedDerivedOfTId);
            await derivedOfTRepository.UnitOfWork.Received(1).SaveChangesAsync();
            DerivedOfTAssertions.AssertEquivalent(testCommand, addedDerivedOfT);
        }
    }
}