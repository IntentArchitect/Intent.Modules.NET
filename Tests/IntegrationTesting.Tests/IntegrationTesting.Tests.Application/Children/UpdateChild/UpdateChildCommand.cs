using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.Children.UpdateChild
{
    public class UpdateChildCommand : IRequest, ICommand
    {
        public UpdateChildCommand(Guid id, string name, Guid myParentId)
        {
            Id = id;
            Name = name;
            MyParentId = myParentId;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MyParentId { get; set; }
    }
}