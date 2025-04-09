using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.People.SetPersons
{
    public class SetPersonsCommand : IRequest<List<Guid>>, ICommand
    {
        public SetPersonsCommand(List<PersonDCDto> people)
        {
            People = people;
        }

        public List<PersonDCDto> People { get; set; }
    }
}