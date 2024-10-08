using System;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.People.UpdatePerson
{
    public class UpdatePersonCommand : IRequest, ICommand
    {
        public UpdatePersonCommand(Guid id, UpdatePersonDetailsDto details)
        {
            Id = id;
            Details = details;
        }

        public Guid Id { get; set; }
        public UpdatePersonDetailsDto Details { get; set; }
    }
}