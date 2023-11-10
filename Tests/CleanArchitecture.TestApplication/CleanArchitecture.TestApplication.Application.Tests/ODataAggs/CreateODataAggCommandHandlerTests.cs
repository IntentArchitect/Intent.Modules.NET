using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ODataAggs.CreateODataAgg;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Application.Tests.ODataQuery.ODataAggs;
using CleanArchitecture.TestApplication.Domain.Entities.ODataQuery;
using CleanArchitecture.TestApplication.Domain.Repositories.ODataQuery;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ODataAggs
{
    public class CreateODataAggCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateODataAggCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsODataAggToRepository(CreateODataAggCommand testCommand)
        {
            // Arrange
            var oDataAggRepository = Substitute.For<IODataAggRepository>();
            var expectedODataAggId = new Fixture().Create<System.Guid>();
            ODataAgg addedODataAgg = null;
            oDataAggRepository.OnAdd(ent => addedODataAgg = ent);
            oDataAggRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedODataAgg.Id = expectedODataAggId);

            var sut = new CreateODataAggCommandHandler(oDataAggRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedODataAggId);
            await oDataAggRepository.UnitOfWork.Received(1).SaveChangesAsync();
            ODataAggAssertions.AssertEquivalent(testCommand, addedODataAgg);
        }
    }
}