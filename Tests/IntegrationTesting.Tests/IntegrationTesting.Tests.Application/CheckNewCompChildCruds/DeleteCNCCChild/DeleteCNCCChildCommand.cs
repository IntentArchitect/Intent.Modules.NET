using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.DeleteCNCCChild
{
    public class DeleteCNCCChildCommand : IRequest, ICommand
    {
        public DeleteCNCCChildCommand(Guid checkNewCompChildCrudId, Guid id)
        {
            CheckNewCompChildCrudId = checkNewCompChildCrudId;
            Id = id;
        }

        public Guid CheckNewCompChildCrudId { get; set; }
        public Guid Id { get; set; }
    }
}