using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.Tests.Extensions;
using CleanArchitecture.TestApplication.Application.VariantTypesClasses.CreateVariantTypesClass;
using CleanArchitecture.TestApplication.Domain.Entities;
using CleanArchitecture.TestApplication.Domain.Repositories;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.VariantTypesClasses
{
    public class CreateVariantTypesClassCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateVariantTypesClassCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsVariantTypesClassToRepository(CreateVariantTypesClassCommand testCommand)
        {
            // Arrange
            var expectedVariantTypesClassId = new Fixture().Create<System.Guid>();

            VariantTypesClass addedVariantTypesClass = null;
            var repository = Substitute.For<IVariantTypesClassRepository>();
            repository.OnAdd(ent => addedVariantTypesClass = ent);
            repository.OnSaveChanges(() => addedVariantTypesClass.Id = expectedVariantTypesClassId);

            var sut = new CreateVariantTypesClassCommandHandler(repository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedVariantTypesClassId);
            await repository.UnitOfWork.Received(1).SaveChangesAsync();
            VariantTypesClassAssertions.AssertEquivalent(testCommand, addedVariantTypesClass);
        }
    }
}