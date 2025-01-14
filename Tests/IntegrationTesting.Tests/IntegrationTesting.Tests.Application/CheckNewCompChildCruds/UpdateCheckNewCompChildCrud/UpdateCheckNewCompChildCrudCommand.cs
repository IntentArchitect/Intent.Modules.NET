using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.CheckNewCompChildCruds.UpdateCheckNewCompChildCrud
{
    public class UpdateCheckNewCompChildCrudCommand : IRequest, ICommand
    {
        public UpdateCheckNewCompChildCrudCommand(string name, Guid id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}