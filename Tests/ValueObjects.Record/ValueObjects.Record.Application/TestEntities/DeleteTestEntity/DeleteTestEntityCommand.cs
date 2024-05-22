using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using ValueObjects.Record.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace ValueObjects.Record.Application.TestEntities.DeleteTestEntity
{
    public class DeleteTestEntityCommand : IRequest, ICommand
    {
        public DeleteTestEntityCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}