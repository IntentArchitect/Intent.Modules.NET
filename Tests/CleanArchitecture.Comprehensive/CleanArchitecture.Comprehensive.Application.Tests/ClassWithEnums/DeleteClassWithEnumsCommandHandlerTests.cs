using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums.DeleteClassWithEnums;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using CleanArchitecture.Comprehensive.Domain.Repositories.Enums;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.ClassWithEnums
{
    public class DeleteClassWithEnumsCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<Domain.Entities.Enums.ClassWithEnums>();
            fixture.Customize<DeleteClassWithEnumsCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteClassWithEnumsCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesClassWithEnumsFromRepository(
            DeleteClassWithEnumsCommand testCommand,
            Domain.Entities.Enums.ClassWithEnums existingEntity)
        {
            // Arrange
            var classWithEnumsRepository = Substitute.For<IClassWithEnumsRepository>();
            classWithEnumsRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new DeleteClassWithEnumsCommandHandler(classWithEnumsRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            classWithEnumsRepository.Received(1).Remove(Arg.Is<Domain.Entities.Enums.ClassWithEnums>(p => testCommand.Id == p.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidClassWithEnumsId_ReturnsNotFound()
        {
            // Arrange
            var classWithEnumsRepository = Substitute.For<IClassWithEnumsRepository>();
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteClassWithEnumsCommand>();
            classWithEnumsRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<Domain.Entities.Enums.ClassWithEnums>(default));


            var sut = new DeleteClassWithEnumsCommandHandler(classWithEnumsRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}