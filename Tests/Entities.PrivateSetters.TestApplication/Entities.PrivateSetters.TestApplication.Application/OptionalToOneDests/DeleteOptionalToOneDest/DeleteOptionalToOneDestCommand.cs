using System;
using Entities.PrivateSetters.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OptionalToOneDests.DeleteOptionalToOneDest
{
    public class DeleteOptionalToOneDestCommand : IRequest, ICommand
    {
        public DeleteOptionalToOneDestCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}