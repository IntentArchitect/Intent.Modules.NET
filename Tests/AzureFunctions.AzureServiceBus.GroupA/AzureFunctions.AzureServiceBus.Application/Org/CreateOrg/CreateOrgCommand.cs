using System;
using System.Collections.Generic;
using AzureFunctions.AzureServiceBus.Application.Common.Interfaces;
using AzureFunctions.AzureServiceBus.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.Org.CreateOrg
{
    public class CreateOrgCommand : IRequest, ICommand
    {
        public CreateOrgCommand(string name, OrgType type, DateTime founded, List<CreateOrgCommandDepartmentsDto> departments)
        {
            Name = name;
            Type = type;
            Founded = founded;
            Departments = departments;
        }

        public string Name { get; set; }
        public OrgType Type { get; set; }
        public DateTime Founded { get; set; }
        public List<CreateOrgCommandDepartmentsDto> Departments { get; set; }
    }
}