using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.WithCompositeKeys;
using CleanArchitecture.TestApplication.Application.WithCompositeKeys.CreateWithCompositeKey;
using CleanArchitecture.TestApplication.Application.WithCompositeKeys.UpdateWithCompositeKey;
using CleanArchitecture.TestApplication.Domain.Entities;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.WithCompositeKeys
{
    public static class WithCompositeKeyAssertions
    {
        public static void AssertEquivalent(CreateWithCompositeKeyCommand expectedDto, WithCompositeKey actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
        }

        public static void AssertEquivalent(WithCompositeKeyDto actualDto, WithCompositeKey expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Key1Id.Should().Be(expectedEntity.Key1Id);
            actualDto.Key2Id.Should().Be(expectedEntity.Key2Id);
            actualDto.Name.Should().Be(expectedEntity.Name);
        }

        public static void AssertEquivalent(UpdateWithCompositeKeyCommand expectedDto, WithCompositeKey actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Name.Should().Be(expectedDto.Name);
            actualEntity.Key1Id.Should().Be(expectedDto.Key1Id);
            actualEntity.Key2Id.Should().Be(expectedDto.Key2Id);
        }
    }
}