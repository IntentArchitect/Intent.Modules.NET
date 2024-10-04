using System;
using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Supers.CreateSuper
{
    public class CreateSuperCommand : IRequest<Guid>, ICommand
    {
        public CreateSuperCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}