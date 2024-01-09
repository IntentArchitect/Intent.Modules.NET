using System;
using Entities.Interfaces.EF.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.People.DeletePerson
{
    public class DeletePersonCommand : IRequest, ICommand
    {
        public DeletePersonCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}