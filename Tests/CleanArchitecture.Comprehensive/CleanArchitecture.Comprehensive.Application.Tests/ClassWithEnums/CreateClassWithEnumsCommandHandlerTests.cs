using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums.CreateClassWithEnums;
using CleanArchitecture.Comprehensive.Application.Tests.Enums.ClassWithEnums;
using CleanArchitecture.Comprehensive.Application.Tests.Extensions;
using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using CleanArchitecture.Comprehensive.Domain.Repositories.Enums;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.ClassWithEnums
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
            var classWithEnumsRepository = Substitute.For<IClassWithEnumsRepository>();
            var expectedClassWithEnumsId = new Fixture().Create<System.Guid>();
            Domain.Entities.Enums.ClassWithEnums addedClassWithEnums = null;
            classWithEnumsRepository.OnAdd(ent => addedClassWithEnums = ent);
            classWithEnumsRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ => addedClassWithEnums.Id = expectedClassWithEnumsId);

            var sut = new CreateClassWithEnumsCommandHandler(classWithEnumsRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedClassWithEnumsId);
            await classWithEnumsRepository.UnitOfWork.Received(1).SaveChangesAsync();
            ClassWithEnumsAssertions.AssertEquivalent(testCommand, addedClassWithEnums);
        }
    }
}