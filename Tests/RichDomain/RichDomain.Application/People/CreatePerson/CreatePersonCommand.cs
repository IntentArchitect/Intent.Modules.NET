using System;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using RichDomain.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace RichDomain.Application.People.CreatePerson
{
    public class CreatePersonCommand : IRequest<Guid>, ICommand
    {
        public CreatePersonCommand(string firstName)
        {
            FirstName = firstName;
        }

        public string FirstName { get; set; }
    }
}