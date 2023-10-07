using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRoot
{
    public class UpdateImplicitKeyAggrRootCommand : IRequest, ICommand
    {
        public UpdateImplicitKeyAggrRootCommand(Guid id,
            string attribute,
            List<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDto> implicitKeyNestedCompositions)
        {
            Id = id;
            Attribute = attribute;
            ImplicitKeyNestedCompositions = implicitKeyNestedCompositions;
        }

        public Guid Id { get; private set; }
        public string Attribute { get; set; }
        public List<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDto> ImplicitKeyNestedCompositions { get; set; }

        public void SetId(Guid id)
        {
            Id = id;
        }
    }
}