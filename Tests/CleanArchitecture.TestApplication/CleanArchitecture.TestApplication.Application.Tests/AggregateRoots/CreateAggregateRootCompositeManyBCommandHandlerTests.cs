using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRoots;
using CleanArchitecture.TestApplication.Application.AggregateRoots.CreateAggregateRootCompositeManyB;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Nested.NestedCreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRoots
{
    public class CreateAggregateRootCompositeManyBCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var testCommand = CreateTestCommand();
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            var owner = fixture.Create<AggregateRoot>();
            testCommand.AggregateRootId = owner.Id;
            yield return new object[] { owner, testCommand };
        }
        
        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsCompositeManyBToRepository(AggregateRoot owner, CreateAggregateRootCompositeManyBCommand testCommand)
        {
            // Arrange
            var expectedAggregateRootId = Guid.NewGuid();

            CompositeManyB addedCompositeManyB = null;
            var repository = Substitute.For<IAggregateRootRepository>();
            repository.FindByIdAsync(testCommand.AggregateRootId, CancellationToken.None).Returns(Task.FromResult(owner));
            repository.OnSaveChanges(
                () =>
                {
                    addedCompositeManyB = owner.Composites.Single(p => p.Id == default);
                    addedCompositeManyB.Id = expectedAggregateRootId;
                    addedCompositeManyB.AggregateRootId = testCommand.AggregateRootId;
                });
            var sut = new CreateAggregateRootCompositeManyBCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedAggregateRootId);
            AggregateRootAssertions.AssertEquivalent(testCommand, addedCompositeManyB);
        }

        private static CreateAggregateRootCompositeManyBCommand CreateTestCommand()
        {
            var testCommand = new CreateAggregateRootCompositeManyBCommand()
            {
                AggregateRootId = new Guid("17A8B8FD-651E-4274-A38F-D18BAD6A9072"),
                CompositeAttr = "Test 8E36FE61-E856-4693-B1B2-FD15C84E489D",
                SomeDate = DateTime.Now,
                Composite = new CreateAggregateRootCompositeManyBCompositeSingleBBDto
                {
                    CompositeAttr = "Test 26F92331-01AA-4AD8-97C2-D9CEB2311118"
                },
                Composites = new List<CreateAggregateRootCompositeManyBCompositeManyBBDto>
                {
                    new()
                    {
                        CompositeAttr = "Test 67484EA2-FC76-49C9-B017-1A1523FDDC48"
                    },
                    new()
                    {
                        CompositeAttr = "Test 6FE6063EE-83AF-4A74-B193-DF209DBE61F6"
                    },
                    new()
                    {
                        CompositeAttr = "Test EA133B76-B8BD-4681-A286-7D67DDC0DE16"
                    }
                }
            };
            return testCommand;
        }
    }
}