using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.UpdateDiffId
{
    public class UpdateDiffIdCommand : IRequest, ICommand
    {
        public UpdateDiffIdCommand(Guid myId, string name)
        {
            MyId = myId;
            Name = name;
        }

        public Guid MyId { get; set; }
        public string Name { get; set; }
    }
}