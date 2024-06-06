using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.UpdateAnemicIntegrationTriggering
{
    public class UpdateAnemicIntegrationTriggeringCommand : IRequest, ICommand
    {
        public UpdateAnemicIntegrationTriggeringCommand(Guid id, string value)
        {
            Id = id;
            Value = value;
        }

        public Guid Id { get; set; }
        public string Value { get; set; }
    }
}