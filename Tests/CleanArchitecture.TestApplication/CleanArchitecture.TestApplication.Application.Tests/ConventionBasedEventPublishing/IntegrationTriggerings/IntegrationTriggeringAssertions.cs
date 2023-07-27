using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.CreateIntegrationTriggering;
using CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.UpdateIntegrationTriggering;
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
            CreateIntegrationTriggeringCommand expectedDto,
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
            UpdateIntegrationTriggeringCommand expectedDto,
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