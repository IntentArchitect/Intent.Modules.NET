using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.DeleteDiffId
{
    public class DeleteDiffIdCommand : IRequest, ICommand
    {
        public DeleteDiffIdCommand(Guid myId)
        {
            MyId = myId;
        }

        public Guid MyId { get; set; }
    }
}