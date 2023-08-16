using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.CreateAnemicIntegrationTriggering;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.UpdateAnemicIntegrationTriggering;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsDdd.CreateDddIntegrationTriggering;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsDdd.UpdateDddIntegrationTriggering;
using CleanArchitecture.TestApplication.Domain.Entities.ConventionBasedEventPublishing;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Tests.ConventionBasedEventPublishing.IntegrationTriggerings
{
    public static class IntegrationTriggeringAssertions
    {
        public static void AssertEquivalent(
            CreateAnemicIntegrationTriggeringCommand expectedDto,
            IntegrationTriggering actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Value.Should().Be(expectedDto.Value);
        }

        public static void AssertEquivalent(
            CreateDddIntegrationTriggeringCommand expectedDto,
            IntegrationTriggering actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
        }

        public static void AssertEquivalent(
            UpdateAnemicIntegrationTriggeringCommand expectedDto,
            IntegrationTriggering actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
            actualEntity.Value.Should().Be(expectedDto.Value);
        }

        public static void AssertEquivalent(
            UpdateDddIntegrationTriggeringCommand expectedDto,
            IntegrationTriggering actualEntity)
        {
            if (expectedDto == null)
            {
                actualEntity.Should().BeNull();
                return;
            }

            actualEntity.Should().NotBeNull();
        }
    }
}