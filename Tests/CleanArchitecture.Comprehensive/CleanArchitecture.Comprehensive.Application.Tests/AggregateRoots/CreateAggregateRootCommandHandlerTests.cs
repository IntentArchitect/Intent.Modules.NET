using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.Comprehensive.Application.Tests.CRUD.AggregateRoots;
using CleanArchitecture.Comprehensive.Application.Tests.Extensions;
using CleanArchitecture.Comprehensive.Domain.Entities.CRUD;
using CleanArchitecture.Comprehensive.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.AggregateRoots
{
    public class CreateAggregateRootCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateAggregateRootCommand>() };

            fixture = new Fixture();
            fixture.Customize<CreateAggregateRootCommand>(comp => comp.Without(x => x.Composite));
            yield return new object[] { fixture.Create<CreateAggregateRootCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsAggregateRootToRepository(CreateAggregateRootCommand testCommand)
        {
            // Arrange
            var aggregateRootRepository = Substitute.For<IAggregateRootRepository>();
            var expectedAggregateRootId = new Fixture().Create<System.Guid>();
            AggregateRoot addedAggregateRoot = null;
            aggregateRootRepository.OnAdd(ent => addedAggregateRoot = ent);
            aggregateRootRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedAggregateRoot.Id = expectedAggregateRootId);

            var sut = new CreateAggregateRootCommandHandler(aggregateRootRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedAggregateRootId);
            await aggregateRootRepository.UnitOfWork.Received(1).SaveChangesAsync();
            AggregateRootAssertions.AssertEquivalent(testCommand, addedAggregateRoot);
        }
    }
}