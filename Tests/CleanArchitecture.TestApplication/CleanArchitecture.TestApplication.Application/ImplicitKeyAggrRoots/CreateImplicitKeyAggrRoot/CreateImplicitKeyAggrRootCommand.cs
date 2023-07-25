using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot
{
    public class CreateImplicitKeyAggrRootCommand : IRequest<Guid>, ICommand
    {
        public CreateImplicitKeyAggrRootCommand(string attribute,
            List<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto> implicitKeyNestedCompositions)
        {
            Attribute = attribute;
            ImplicitKeyNestedCompositions = implicitKeyNestedCompositions;
        }

        public string Attribute { get; set; }
        public List<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDto> ImplicitKeyNestedCompositions { get; set; }
    }
}