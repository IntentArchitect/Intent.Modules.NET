using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.IntegrationTriggeringsAnemic.CreateIntegrationTriggering
{
    public class CreateIntegrationTriggeringCommand : IRequest<Guid>, ICommand
    {
        public CreateIntegrationTriggeringCommand(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}