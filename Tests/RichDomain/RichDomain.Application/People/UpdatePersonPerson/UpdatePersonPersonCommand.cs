using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RichDomain.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace RichDomain.Application.People.UpdatePersonPerson
{
    public class UpdatePersonPersonCommand : IRequest, ICommand
    {
        public UpdatePersonPersonCommand(Guid id, string firstName)
        {
            Id = id;
            FirstName = firstName;
        }

        public Guid Id { get; set; }
        public string FirstName { get; set; }
    }
}