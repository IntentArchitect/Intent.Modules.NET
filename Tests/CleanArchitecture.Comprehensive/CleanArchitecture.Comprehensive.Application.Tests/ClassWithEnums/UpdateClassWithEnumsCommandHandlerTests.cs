using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.ClassWithEnums.UpdateClassWithEnums;
using CleanArchitecture.Comprehensive.Application.Tests.Enums.ClassWithEnums;
using CleanArchitecture.Comprehensive.Domain.Common;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Entities.Enums;
using CleanArchitecture.Comprehensive.Domain.Repositories.Enums;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.ClassWithEnums
{
    public class UpdateClassWithEnumsCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<Domain.Entities.Enums.ClassWithEnums>();
            fixture.Customize<UpdateClassWithEnumsCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateClassWithEnumsCommand testCommand,
            Domain.Entities.Enums.ClassWithEnums existingEntity)
        {
            // Arrange
            var classWithEnumsRepository = Substitute.For<IClassWithEnumsRepository>();
            classWithEnumsRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new UpdateClassWithEnumsCommandHandler(classWithEnumsRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            ClassWithEnumsAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateClassWithEnumsCommand>();
            var classWithEnumsRepository = Substitute.For<IClassWithEnumsRepository>();
            classWithEnumsRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<Domain.Entities.Enums.ClassWithEnums>(default));


            var sut = new UpdateClassWithEnumsCommandHandler(classWithEnumsRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}