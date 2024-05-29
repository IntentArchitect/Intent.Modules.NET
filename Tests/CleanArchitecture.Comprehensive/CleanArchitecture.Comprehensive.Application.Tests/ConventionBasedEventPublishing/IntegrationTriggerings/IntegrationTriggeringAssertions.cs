using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.CreateAnemicIntegrationTriggering;
using CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.UpdateAnemicIntegrationTriggering;
using CleanArchitecture.Comprehensive.Domain.Entities.ConventionBasedEventPublishing;
using FluentAssertions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CRUD.Tests.Assertions.AssertionClass", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Tests.ConventionBasedEventPublishing.IntegrationTriggerings
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
    }
}