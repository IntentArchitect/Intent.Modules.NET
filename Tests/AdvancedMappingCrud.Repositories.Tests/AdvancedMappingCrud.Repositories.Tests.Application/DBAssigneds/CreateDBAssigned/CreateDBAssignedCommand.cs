using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.DBAssigneds.CreateDBAssigned
{
    public class CreateDBAssignedCommand : IRequest<Guid>, ICommand
    {
        public CreateDBAssignedCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}