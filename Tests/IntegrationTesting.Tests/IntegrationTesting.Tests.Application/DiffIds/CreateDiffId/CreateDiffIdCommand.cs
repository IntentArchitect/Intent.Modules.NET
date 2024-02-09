using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.DiffIds.CreateDiffId
{
    public class CreateDiffIdCommand : IRequest<Guid>, ICommand
    {
        public CreateDiffIdCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}