using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.OperationOneToManySource
{
    public class OperationOneToManySourceCommand : IRequest, ICommand
    {
        public OperationOneToManySourceCommand(Guid id, string attribute)
        {
            Id = id;
            Attribute = attribute;
        }

        public string Attribute { get; set; }
        public Guid Id { get; set; }
    }
}