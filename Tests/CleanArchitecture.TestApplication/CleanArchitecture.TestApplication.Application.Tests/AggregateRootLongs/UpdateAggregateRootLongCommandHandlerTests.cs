using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.UpdateAggregateRootLong;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
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
        [Theory]
        [MemberData(nameof(GetValidTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(UpdateAggregateRootLongCommand testCommand, AggregateRootLong existingEntity)
        {
            // Arrange
            var expectedAggregateRootLong = CreateExpectedAggregateRootLong(testCommand);

            var repository = Substitute.For<IAggregateRootLongRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult(existingEntity));

            var sut = new UpdateAggregateRootLongCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            expectedAggregateRootLong.Should().BeEquivalentTo(existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateAggregateRootLongCommand>();

            var repository = Substitute.For<IAggregateRootLongRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<AggregateRootLong>(null));

            var sut = new UpdateAggregateRootLongCommandHandler(repository);

            // Act
            // Assert
            await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await sut.Handle(testCommand, CancellationToken.None);
            });
        }

        public static IEnumerable<object[]> GetValidTestData()
        {
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateAggregateRootLongCommand>();
            yield return new object[] { testCommand, CreateExpectedAggregateRootLong(testCommand) };

            fixture = new Fixture();
            fixture.Customize<UpdateAggregateRootLongCommand>(comp => comp.Without(x => x.CompositeOfAggrLong));
            testCommand = fixture.Create<UpdateAggregateRootLongCommand>();
            yield return new object[] { testCommand, CreateExpectedAggregateRootLong(testCommand) };
        }

        private static AggregateRootLong CreateExpectedAggregateRootLong(UpdateAggregateRootLongCommand dto)
        {
            return new AggregateRootLong
            {
                Attribute = dto.Attribute,
                CompositeOfAggrLong = dto.CompositeOfAggrLong != null ? CreateExpectedCompositeOfAggrLong(dto.CompositeOfAggrLong) : null,
            };
        }

        private static CompositeOfAggrLong CreateExpectedCompositeOfAggrLong(UpdateAggregateRootLongCompositeOfAggrLongDto dto)
        {
            return new CompositeOfAggrLong
            {
                Attribute = dto.Attribute,
            };
        }
    }
}