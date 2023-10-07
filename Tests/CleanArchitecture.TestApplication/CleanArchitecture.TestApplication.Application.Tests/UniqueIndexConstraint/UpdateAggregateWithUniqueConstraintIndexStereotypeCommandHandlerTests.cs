using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.Tests.UniqueIndexConstraint.AggregateWithUniqueConstraintIndexStereotypes;
using CleanArchitecture.TestApplication.Application.UniqueIndexConstraint.UpdateAggregateWithUniqueConstraintIndexStereotype;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.UniqueIndexConstraint;
using CleanArchitecture.TestApplication.Domain.Repositories.UniqueIndexConstraint;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.UpdateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.UniqueIndexConstraint
{
    public class UpdateAggregateWithUniqueConstraintIndexStereotypeCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null!);
            var existingEntity = fixture.Create<AggregateWithUniqueConstraintIndexStereotype>();
            fixture.Customize<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>(comp => comp.Do(x => x.SetId(existingEntity.Id)));
            var testCommand = fixture.Create<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_UpdatesExistingEntity(
            UpdateAggregateWithUniqueConstraintIndexStereotypeCommand testCommand,
            AggregateWithUniqueConstraintIndexStereotype existingEntity)
        {
            // Arrange
            var aggregateWithUniqueConstraintIndexStereotypeRepository = Substitute.For<IAggregateWithUniqueConstraintIndexStereotypeRepository>();
            aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult(existingEntity));

            var sut = new UpdateAggregateWithUniqueConstraintIndexStereotypeCommandHandler(aggregateWithUniqueConstraintIndexStereotypeRepository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            AggregateWithUniqueConstraintIndexStereotypeAssertions.AssertEquivalent(testCommand, existingEntity);
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<UpdateAggregateWithUniqueConstraintIndexStereotypeCommand>();
            var aggregateWithUniqueConstraintIndexStereotypeRepository = Substitute.For<IAggregateWithUniqueConstraintIndexStereotypeRepository>();
            aggregateWithUniqueConstraintIndexStereotypeRepository.FindByIdAsync(testCommand.Id, CancellationToken.None)!.Returns(Task.FromResult<AggregateWithUniqueConstraintIndexStereotype>(default));


            var sut = new UpdateAggregateWithUniqueConstraintIndexStereotypeCommandHandler(aggregateWithUniqueConstraintIndexStereotypeRepository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}