using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.UpdateAggregateRootLong;
using CleanArchitecture.TestApplication.Application.Tests.CRUD.AggregateRootLongs;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.CRUD;
using CleanArchitecture.TestApplication.Domain.Repositories.CRUD;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRootLongs
{
    public class UpdateAggregateRootLongCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<AggregateRootLong>();
            fixture.Customize<UpdateAggregateRootLongCommand>(comp => comp.Do(x => x.SetId(existingEntity.Id)));
            var testCommand = fixture.Create<UpdateAggregateRootLongCommand>();
            yield return new object[] { testCommand, existingEntity };

            fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            existingEntity = fixture.Create<AggregateRootLong>();
            fixture.Customize<UpdateAggregateRootLongCommand>(comp => comp.Without(x => x.CompositeOfAggrLong).Do(x => x.SetId(existingEntity.Id)));
            testCommand = fixture.Create<UpdateAggregateRootLongCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateAggregateRootLongCommand testCommand,
            AggregateRootLong existingEntity)
        {
            // Arrange
            var aggregateRootLongRepository = Substitute.For<IAggregateRootLongRepository>();
            aggregateRootLongRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new UpdateAggregateRootLongCommandHandler(aggregateRootLongRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            AggregateRootLongAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateAggregateRootLongCommand>();
            var aggregateRootLongRepository = Substitute.For<IAggregateRootLongRepository>();
            aggregateRootLongRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<AggregateRootLong>(default));


            var sut = new UpdateAggregateRootLongCommandHandler(aggregateRootLongRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}