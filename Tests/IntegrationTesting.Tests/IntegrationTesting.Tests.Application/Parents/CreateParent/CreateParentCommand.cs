using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Parents.CreateParent
{
    public class CreateParentCommand : IRequest<Guid>, ICommand
    {
        public CreateParentCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}