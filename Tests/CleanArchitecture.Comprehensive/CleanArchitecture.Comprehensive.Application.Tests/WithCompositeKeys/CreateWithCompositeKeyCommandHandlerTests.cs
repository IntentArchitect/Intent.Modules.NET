using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using CleanArchitecture.Comprehensive.Application.Tests.CompositeKeys.WithCompositeKeys;
using CleanArchitecture.Comprehensive.Application.Tests.Extensions;
using CleanArchitecture.Comprehensive.Application.WithCompositeKeys.CreateWithCompositeKey;
using CleanArchitecture.Comprehensive.Domain.Entities.CompositeKeys;
using CleanArchitecture.Comprehensive.Domain.Repositories.CompositeKeys;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;
using NSubstitute;
using Xunit;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Owner.CreateCommandHandlerTests", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.WithCompositeKeys
{
    public class CreateWithCompositeKeyCommandHandlerTests
    {
        public static IEnumerable<object[]> GetSuccessfulResultTestData()
        {
            var fixture = new Fixture();
            yield return new object[] { fixture.Create<CreateWithCompositeKeyCommand>() };
        }

        [Theory]
        [MemberData(nameof(GetSuccessfulResultTestData))]
        public async Task Handle_WithValidCommand_AddsWithCompositeKeyToRepository(CreateWithCompositeKeyCommand testCommand)
        {
            // Arrange
            var withCompositeKeyRepository = Substitute.For<IWithCompositeKeyRepository>();
            var expectedWithCompositeKeyKey1Id = new Fixture().Create<System.Guid>();
            var expectedWithCompositeKeyKey2Id = new Fixture().Create<System.Guid>();
            WithCompositeKey addedWithCompositeKey = null;
            withCompositeKeyRepository.OnAdd(ent => addedWithCompositeKey = ent);
            withCompositeKeyRepository.UnitOfWork
                .When(async x => await x.SaveChangesAsync(CancellationToken.None))
                .Do(_ =>
                {
                    addedWithCompositeKey.Key1Id = expectedWithCompositeKeyKey1Id;
                    addedWithCompositeKey.Key2Id = expectedWithCompositeKeyKey2Id;
                });

            var sut = new CreateWithCompositeKeyCommandHandler(withCompositeKeyRepository);

            // Act
            var result = await sut.Handle(testCommand, CancellationToken.None);

            // Assert
            result.Should().Be(expectedWithCompositeKeyKey1Id);
            await withCompositeKeyRepository.UnitOfWork.Received(1).SaveChangesAsync();
            WithCompositeKeyAssertions.AssertEquivalent(testCommand, addedWithCompositeKey);
        }
    }
}