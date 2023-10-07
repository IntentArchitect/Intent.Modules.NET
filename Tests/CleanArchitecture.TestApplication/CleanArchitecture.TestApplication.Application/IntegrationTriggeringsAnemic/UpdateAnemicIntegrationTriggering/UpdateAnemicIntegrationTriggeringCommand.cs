using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.UpdateAnemicIntegrationTriggering
{
    public class UpdateAnemicIntegrationTriggeringCommand : IRequest, ICommand
    {
        public UpdateAnemicIntegrationTriggeringCommand(Guid id, string value)
        {
            Id = id;
            Value = value;
        }

        public Guid Id { get; private set; }
        public string Value { get; set; }

        public void SetId(Guid id)
        {
            if (Id == default)
            {
                Id = id;
            }
        }
    }
}