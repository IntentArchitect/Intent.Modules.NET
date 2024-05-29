using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsDdd.UpdateDddIntegrationTriggering
{
    public class UpdateDddIntegrationTriggeringCommand : IRequest, ICommand
    {
        public UpdateDddIntegrationTriggeringCommand(string value, Guid id)
        {
            Value = value;
            Id = id;
        }

        public string Value { get; set; }
        public Guid Id { get; set; }
    }
}