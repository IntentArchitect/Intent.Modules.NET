using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps.UpdateHasMissingDep
{
    public class UpdateHasMissingDepCommand : IRequest, ICommand
    {
        public UpdateHasMissingDepCommand(string name, Guid missingDepId, Guid id)
        {
            Name = name;
            MissingDepId = missingDepId;
            Id = id;
        }

        public string Name { get; set; }
        public Guid MissingDepId { get; set; }
        public Guid Id { get; set; }
    }
}