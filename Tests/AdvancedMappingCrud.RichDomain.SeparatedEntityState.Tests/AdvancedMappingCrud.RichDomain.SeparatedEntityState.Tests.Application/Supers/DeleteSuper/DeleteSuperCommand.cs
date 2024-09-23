using System;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Supers.DeleteSuper
{
    public class DeleteSuperCommand : IRequest, ICommand
    {
        public DeleteSuperCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}