using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.CreateSubmission;
using CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.UpdateSubmission;
using CleanArchitecture.Comprehensive.Domain.BasicMappingMapToValueObjects;
using CleanArchitecture.Comprehensive.Domain.Entities.BasicMappingMapToValueObjects;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.BasicMappingMapToValueObjects.Submissions
{
    public static class SubmissionAssertions
    {
        public static void AssertEquivalent(
            IEnumerable<SubmissionDto> actualDtos,
            IEnumerable<Submission> expectedEntities)
        {
            if (expectedEntities == null)
            {
                actualDtos.Should().BeNullOrEmpty();
                return;
            }

            actualDtos.Should().HaveSameCount(actualDtos);
            for (int i = 0; i < expectedEntities.Count(); i++)
            {
                var entity = expectedEntities.ElementAt(i);
                var dto = actualDtos.ElementAt(i);
                if (entity == null)
                {
                    dto.Should().BeNull();
                    continue;
                }

                dto.Should().NotBeNull();
                dto.Id.Should().Be(entity.Id);
                dto.SubmissionType.Should().Be(entity.SubmissionType);
            }
        }

        public static void AssertEquivalent(SubmissionDto actualDto, Submission expectedEntity)
        {
            if (expectedEntity == null)
            {
                actualDto.Should().BeNull();
                return;
            }

            actualDto.Should().NotBeNull();
            actualDto.Id.Should().Be(expectedEntity.Id);
            actualDto.SubmissionType.Should().Be(expectedEntity.SubmissionType);
        }

        public static void AssertEquivalent(UpdateSubmissionCommand expectedDto, Submission actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.SubmissionType.Should().Be(expectedDto.SubmissionType);
            AssertEquivalent(expectedDto.Items, actualEntity.Items);
        }

        public static void AssertEquivalent(
            IEnumerable<UpdateSubmissionItemDto> expectedDtos,
            IEnumerable<Item> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.Key.Should().Be(dto.Key);
                entity.Value.Should().Be(dto.Value);
            }
        }

        public static void AssertEquivalent(CreateSubmissionCommand expectedDto, Submission actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.SubmissionType.Should().Be(expectedDto.SubmissionType);
            AssertEquivalent(expectedDto.Items, actualEntity.Items);
        }

        public static void AssertEquivalent(
            IEnumerable<CreateSubmissionItemDto> expectedDtos,
            IEnumerable<Item> actualEntities)
        {
            if (expectedDtos == null)
            {
                actualEntities.Should().BeNullOrEmpty();
                return;
            }

            actualEntities.Should().HaveSameCount(expectedDtos);
            for (int i = 0; i < expectedDtos.Count(); i++)
            {
                var dto = expectedDtos.ElementAt(i);
                var entity = actualEntities.ElementAt(i);
                if (dto == null)
                {
                    entity.Should().BeNull();
                    continue;
                }

                entity.Should().NotBeNull();
                entity.Key.Should().Be(dto.Key);
                entity.Value.Should().Be(dto.Value);
            }
        }
    }
}