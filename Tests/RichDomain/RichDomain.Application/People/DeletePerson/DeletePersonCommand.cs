using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RichDomain.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace RichDomain.Application.People.DeletePerson
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