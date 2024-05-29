using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.CreateAnemicIntegrationTriggering
{
    public class CreateAnemicIntegrationTriggeringCommand : IRequest<Guid>, ICommand
    {
        public CreateAnemicIntegrationTriggeringCommand(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}