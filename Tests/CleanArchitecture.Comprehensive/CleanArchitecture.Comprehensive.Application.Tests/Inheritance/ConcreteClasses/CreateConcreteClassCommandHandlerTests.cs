using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.CreateConcreteClass;
using CleanArchitecture.Comprehensive.Application.Tests.Extensions;
using CleanArchitecture.Comprehensive.Domain.Entities.Inheritance;
using CleanArchitecture.Comprehensive.Domain.Repositories.Inheritance;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.Inheritance.ConcreteClasses
{
    public class CreateConcreteClassCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateConcreteClassCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsConcreteClassToRepository(CreateConcreteClassCommand testCommand)
        {
            // Arrange
            var concreteClassRepository = Substitute.For<IConcreteClassRepository>();
            var expectedConcreteClassId = new Fixture().Create<System.Guid>();
            ConcreteClass addedConcreteClass = null;
            concreteClassRepository.OnAdd(ent => addedConcreteClass = ent);
            concreteClassRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedConcreteClass.Id = expectedConcreteClassId);

            var sut = new CreateConcreteClassCommandHandler(concreteClassRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedConcreteClassId);
            await concreteClassRepository.UnitOfWork.Received(1).SaveChangesAsync();
            ConcreteClassAssertions.AssertEquivalent(testCommand, addedConcreteClass);
        }
    }
}