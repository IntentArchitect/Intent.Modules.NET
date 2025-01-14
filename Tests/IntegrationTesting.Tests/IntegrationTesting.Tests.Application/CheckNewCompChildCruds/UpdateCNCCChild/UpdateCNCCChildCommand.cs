using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.UpdateCNCCChild
{
    public class UpdateCNCCChildCommand : IRequest, ICommand
    {
        public UpdateCNCCChildCommand(Guid checkNewCompChildCrudId, string description, Guid id)
        {
            CheckNewCompChildCrudId = checkNewCompChildCrudId;
            Description = description;
            Id = id;
        }

        public Guid CheckNewCompChildCrudId { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }
    }
}