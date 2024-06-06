using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.TestNullablities.UpdateTestNullablity;
using CleanArchitecture.Comprehensive.Application.Tests.Nullability.TestNullablities;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.Nullability;
using CleanArchitecture.Comprehensive.Domain.Repositories.Nullability;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.TestNullablities
{
    public class UpdateTestNullablityCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<TestNullablity>();
            fixture.Customize<UpdateTestNullablityCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateTestNullablityCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateTestNullablityCommand testCommand,
            TestNullablity existingEntity)
        {
            // Arrange
            var testNullablityRepository = Substitute.For<ITestNullablityRepository>();
            testNullablityRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new UpdateTestNullablityCommandHandler(testNullablityRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            TestNullablityAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateTestNullablityCommand>();
            var testNullablityRepository = Substitute.For<ITestNullablityRepository>();
            testNullablityRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<TestNullablity>(default));


            var sut = new UpdateTestNullablityCommandHandler(testNullablityRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}