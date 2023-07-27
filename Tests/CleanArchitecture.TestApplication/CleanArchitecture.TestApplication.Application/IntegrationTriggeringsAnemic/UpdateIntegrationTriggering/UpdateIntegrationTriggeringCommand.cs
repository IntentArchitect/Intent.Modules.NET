using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.UpdateIntegrationTriggering
{
    public class UpdateIntegrationTriggeringCommand : IRequest, ICommand
    {
        public UpdateIntegrationTriggeringCommand(Guid id, string value)
        {
            Id = id;
            Value = value;
        }

        public Guid Id { get; set; }
        public string Value { get; set; }
    }
}