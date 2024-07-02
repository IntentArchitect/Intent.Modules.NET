using System;
using IntegrationTesting.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace IntegrationTesting.Tests.Application.HasMissingDeps.CreateHasMissingDep
{
    public class CreateHasMissingDepCommand : IRequest<Guid>, ICommand
    {
        public CreateHasMissingDepCommand(string name, Guid missingDepId)
        {
            Name = name;
            MissingDepId = missingDepId;
        }

        public string Name { get; set; }
        public Guid MissingDepId { get; set; }
    }
}