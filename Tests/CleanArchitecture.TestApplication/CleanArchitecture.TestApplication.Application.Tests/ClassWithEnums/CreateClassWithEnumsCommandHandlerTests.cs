using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.ClassWithEnums.CreateClassWithEnums;
using CleanArchitecture.TestApplication.Application.Tests.Enums.ClassWithEnums;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Domain.Entities.Enums;
using CleanArchitecture.TestApplication.Domain.Repositories.Enums;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ClassWithEnums
{
    public class CreateClassWithEnumsCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateClassWithEnumsCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsClassWithEnumsToRepository(CreateClassWithEnumsCommand testCommand)
        {
            // Arrange
            var expectedClassWithEnumsId = new Fixture().Create<System.Guid>();
            Domain.Entities.Enums.ClassWithEnums addedClassWithEnums = null;
            var repository = Substitute.For<IClassWithEnumsRepository>();
            repository.OnAdd(ent => addedClassWithEnums = ent);
            repository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedClassWithEnums.Id = expectedClassWithEnumsId);
            var sut = new CreateClassWithEnumsCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedClassWithEnumsId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            ClassWithEnumsAssertions.AssertEquivalent(testCommand, addedClassWithEnums);
        }
    }
}