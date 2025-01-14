using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.CreateCNCCChild
{
    public class CreateCNCCChildCommand : IRequest<Guid>, ICommand
    {
        public CreateCNCCChildCommand(Guid checkNewCompChildCrudId, string description)
        {
            CheckNewCompChildCrudId = checkNewCompChildCrudId;
            Description = description;
        }

        public Guid CheckNewCompChildCrudId { get; set; }
        public string Description { get; set; }
    }
}