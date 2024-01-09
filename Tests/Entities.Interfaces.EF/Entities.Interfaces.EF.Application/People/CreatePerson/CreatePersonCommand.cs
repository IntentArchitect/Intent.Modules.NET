using System;
using Entities.Interfaces.EF.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.Interfaces.EF.Application.People.CreatePerson
{
    public class CreatePersonCommand : IRequest<Guid>, ICommand
    {
        public CreatePersonCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}