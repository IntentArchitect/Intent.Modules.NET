using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Parents.DeleteParent
{
    public class DeleteParentCommand : IRequest, ICommand
    {
        public DeleteParentCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}