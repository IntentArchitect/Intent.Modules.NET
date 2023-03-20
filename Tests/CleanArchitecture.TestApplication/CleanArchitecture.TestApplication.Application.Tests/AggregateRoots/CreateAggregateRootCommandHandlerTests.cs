using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRoot;
using CleanArchitecture.TestApplication.Application.Common.Behaviours;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using FluentAssertions.Primitives;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class CreateAggregateRootCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<CreateAggregateRootCommand>();
            yield return new object[] { testCommand };
        }
        
        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsAggregateRootToRepository(CreateAggregateRootCommand testCommand)
        {
            // Arrange
            var expectedAggregateRootId = Guid.NewGuid();
            
            AggregateRoot addedAggregateRoot = null;
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.OnAdd(ent => addedAggregateRoot = ent);
            repository.OnSaveChanges(() => addedAggregateRoot.Id = expectedAggregateRootId);

            var sut = new CreateAggregateRootCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);
            
            // Assert
            result.Should().Be(expectedAggregateRootId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            addedAggregateRoot.Should().BeEquivalentTo(testCommand, c => c
                .Excluding(x => x.Aggregate));

            AggregateRootAssertions.AssertEquivalent(testCommand, addedAggregateRoot);
        }
    }
}