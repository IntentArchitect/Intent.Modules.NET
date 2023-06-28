using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.TestApplication.Application.CustomerCTS.DeleteCustomerCT;
using CleanArchitecture.TestApplication.Domain.Common;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Entities.ComplexTypes;
using CleanArchitecture.TestApplication.Domain.Repositories.ComplexTypes;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.DeleteCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.CustomerCTS
{
    public class DeleteCustomerCTCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            fixture.Register<DomainEvent>(() => null);
            fixture.Customize<CustomerCT>(comp => comp.Without(x => x.DomainEvents));
            var existingEntity = fixture.Create<CustomerCT>();
            fixture.Customize<DeleteCustomerCTCommand>(comp => comp.With(x => x.Id, existingEntity.Id));
            var testCommand = fixture.Create<DeleteCustomerCTCommand>();
            yield return new object[] { testCommand, existingEntity };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_DeletesCustomerCTFromRepository(
            DeleteCustomerCTCommand testCommand,
            CustomerCT existingEntity)
        {
            // Arrange
            var repository = Substitute.For<ICustomerCTRepository>();
            repository.FindByIdAsync(testCommand.Id).Returns(Task.FromResult(existingEntity));

            var sut = new DeleteCustomerCTCommandHandler(repository);

            // Act
            await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            repository.Received(1).Remove(Arg.Is<CustomerCT>(p => p.Id == testCommand.Id));
        }

        [Fact]
        public async Task Handle_WithInvalidIdCommand_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            var testCommand = fixture.Create<DeleteCustomerCTCommand>();

            var repository = Substitute.For<ICustomerCTRepository>();
            repository.FindByIdAsync(testCommand.Id, CancellationToken.None).Returns(Task.FromResult<CustomerCT>(default));
            repository.When(x => x.Remove(null)).Throw(new ArgumentNullException());

            var sut = new DeleteCustomerCTCommandHandler(repository);

            // Act
            var act = async () => await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}