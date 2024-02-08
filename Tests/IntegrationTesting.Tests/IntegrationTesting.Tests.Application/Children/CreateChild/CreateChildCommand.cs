using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Children.CreateChild
{
    public class CreateChildCommand : IRequest<Guid>, ICommand
    {
        public CreateChildCommand(string name, Guid myParentId)
        {
            Name = name;
            MyParentId = myParentId;
        }

        public string Name { get; set; }
        public Guid MyParentId { get; set; }
    }
}