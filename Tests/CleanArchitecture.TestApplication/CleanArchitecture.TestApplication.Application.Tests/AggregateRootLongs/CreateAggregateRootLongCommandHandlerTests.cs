using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.CreateAggregateRootLong;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRootLongs
{
    public class CreateAggregateRootLongCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateAggregateRootLongCommand>() };

            fixture = new Fixture();
            fixture.Customize<CreateAggregateRootLongCommand>(comp => comp.Without(x => x.CompositeOfAggrLong));
            yield return new object[] { fixture.Create<CreateAggregateRootLongCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsAggregateRootLongToRepository(CreateAggregateRootLongCommand testCommand)
        {
            // Arrange
            var aggregateRootLongRepository = Substitute.For<IAggregateRootLongRepository>();
            var expectedAggregateRootLongId = new Fixture().Create<long>();
            AggregateRootLong addedAggregateRootLong = null;
            aggregateRootLongRepository.OnAdd(ent => addedAggregateRootLong = ent);
            aggregateRootLongRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedAggregateRootLong.Id = expectedAggregateRootLongId);

            var sut = new CreateAggregateRootLongCommandHandler(aggregateRootLongRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedAggregateRootLongId);
            await aggregateRootLongRepository.UnitOfWork.Received(1).SaveChangesAsync();
            AggregateRootLongAssertions.AssertEquivalent(testCommand, addedAggregateRootLong);
        }
    }
}