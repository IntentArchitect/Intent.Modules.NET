using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationTriggeringsDdd.UpdateIntegrationTriggering
{
    public class UpdateIntegrationTriggeringCommand : IRequest, ICommand
    {
        public UpdateIntegrationTriggeringCommand(string value, Guid id)
        {
            Value = value;
            Id = id;
        }

        public string Value { get; set; }
        public Guid Id { get; set; }
    }
}