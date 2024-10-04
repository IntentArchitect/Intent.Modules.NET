using System;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.People.CreatePerson
{
    public class CreatePersonCommand : IRequest<Guid>, ICommand
    {
        public CreatePersonCommand(CreatePersonPersonDetailsDto details)
        {
            Details = details;
        }

        public CreatePersonPersonDetailsDto Details { get; set; }
    }
}