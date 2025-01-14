using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.CreateCheckNewCompChildCrud
{
    public class CreateCheckNewCompChildCrudCommand : IRequest<Guid>, ICommand
    {
        public CreateCheckNewCompChildCrudCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}