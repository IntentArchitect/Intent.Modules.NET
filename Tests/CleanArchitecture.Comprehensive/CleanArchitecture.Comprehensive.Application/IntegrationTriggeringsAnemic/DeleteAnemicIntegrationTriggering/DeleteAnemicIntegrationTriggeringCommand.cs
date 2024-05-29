using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationTriggeringsAnemic.DeleteAnemicIntegrationTriggering
{
    public class DeleteAnemicIntegrationTriggeringCommand : IRequest, ICommand
    {
        public DeleteAnemicIntegrationTriggeringCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}