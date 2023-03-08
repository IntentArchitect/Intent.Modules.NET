using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs;
using CleanArchitecture.TestApplication.Application.AggregateRootLongs.CreateAggregateRootLong;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.AggregateRootLongs
{
    public class CreateAggregateRootLongCommandHandlerTests
    {
        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Handle_WithValidCommand_AddsAggregateRootLongToRepository(CreateAggregateRootLongCommand testCommand)
        {
            // Arrange
            var expectedAggregateRootLong = CreateExpectedAggregateRootLong(testCommand);

            AggregateRootLong addedAggregateRootLong = null;
            var repository = Substitute.For<IAggregateRootLongRepository>();
            repository.OnAdd(ent => addedAggregateRootLong = ent);
            repository.OnSave(() => addedAggregateRootLong.Id = expectedAggregateRootLong.Id);

            var sut = new CreateAggregateRootLongCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedAggregateRootLong.Id);
            expectedAggregateRootLong.Should().BeEquivalentTo(addedAggregateRootLong);
        }

        public static IEnumerable<object[]> GetTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateAggregateRootLongCommand>() };

            fixture = new Fixture();
            fixture.Customize<CreateAggregateRootLongCommand>(comp => comp.Without(x => x.CompositeOfAggrLong));
            yield return new object[] { fixture.Create<CreateAggregateRootLongCommand>() };
        }

        private static AggregateRootLong CreateExpectedAggregateRootLong(CreateAggregateRootLongCommand dto)
        {
            return new AggregateRootLong
            {
                Attribute = dto.Attribute,
                CompositeOfAggrLong = dto.CompositeOfAggrLong != null ? CreateExpectedCompositeOfAggrLong(dto.CompositeOfAggrLong) : null,
            };
        }

        private static CompositeOfAggrLong CreateExpectedCompositeOfAggrLong(CreateAggregateRootLongCompositeOfAggrLongDto dto)
        {
            return new CompositeOfAggrLong
            {
                Attribute = dto.Attribute,
            };
        }
    }
}