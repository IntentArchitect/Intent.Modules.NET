using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.PartialCruds.CreatePartialCrud
{
    public class CreatePartialCrudCommand : IRequest<Guid>, ICommand
    {
        public CreatePartialCrudCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}