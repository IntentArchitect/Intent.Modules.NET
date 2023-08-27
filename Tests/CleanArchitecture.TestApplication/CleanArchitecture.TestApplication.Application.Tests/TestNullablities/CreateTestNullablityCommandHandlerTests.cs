using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.TestNullablities.CreateTestNullablity;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Application.Tests.Nullability.TestNullablities;
using CleanArchitecture.TestApplication.Domain.Entities.Nullability;
using CleanArchitecture.TestApplication.Domain.Repositories.Nullability;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.TestNullablities
{
    public class CreateTestNullablityCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateTestNullablityCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsTestNullablityToRepository(CreateTestNullablityCommand testCommand)
        {
            // Arrange
            var testNullablityRepository = Substitute.For<ITestNullablityRepository>();
            var expectedTestNullablityId = new Fixture().Create<System.Guid>();
            TestNullablity addedTestNullablity = null;
            testNullablityRepository.OnAdd(ent => addedTestNullablity = ent);
            testNullablityRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedTestNullablity.Id = expectedTestNullablityId);

            var sut = new CreateTestNullablityCommandHandler(testNullablityRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedTestNullablityId);
            await testNullablityRepository.UnitOfWork.Received(1).SaveChangesAsync();
            TestNullablityAssertions.AssertEquivalent(testCommand, addedTestNullablity);
        }
    }
}