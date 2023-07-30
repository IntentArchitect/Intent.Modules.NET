using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests.OperationAsync
{
    public class OperationAsyncCommand : IRequest, ICommand
    {
        public OperationAsyncCommand(string attribute, Guid id)
        {
            Attribute = attribute;
            Id = id;
        }

        public string Attribute { get; set; }
        public Guid Id { get; set; }
    }
}