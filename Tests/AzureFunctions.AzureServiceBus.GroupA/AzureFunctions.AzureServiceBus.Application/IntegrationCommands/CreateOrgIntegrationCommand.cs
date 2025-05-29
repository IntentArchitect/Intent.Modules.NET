using System;
using System.Collections.Generic;
using AzureFunctions.AzureServiceBus.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationCommand", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Eventing.Messages
{
    public record CreateOrgIntegrationCommand
    {
        public CreateOrgIntegrationCommand()
        {
            Name = null!;
            Departments = null!;
        }

        public string Name { get; init; }
        public OrgType Type { get; init; }
        public DateTime Founded { get; init; }
        public List<OrgDepartmentDto> Departments { get; init; }
    }
}