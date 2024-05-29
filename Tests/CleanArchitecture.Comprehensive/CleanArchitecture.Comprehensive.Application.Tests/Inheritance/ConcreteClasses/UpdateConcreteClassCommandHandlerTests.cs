using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.UpdateConcreteClass;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.Inheritance.ConcreteClasses
{
    public class UpdateConcreteClassCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<ConcreteClass>();
            fixture.Customize<UpdateConcreteClassCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateConcreteClassCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateConcreteClassCommand testCommand,
            ConcreteClass existingEntity)
        {
            // Arrange
            var concreteClassRepository = Substitute.For<IConcreteClassRepository>();
            concreteClassRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new UpdateConcreteClassCommandHandler(concreteClassRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            ConcreteClassAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateConcreteClassCommand>();
            var concreteClassRepository = Substitute.For<IConcreteClassRepository>();
            concreteClassRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<ConcreteClass>(default));


            var sut = new UpdateConcreteClassCommandHandler(concreteClassRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}