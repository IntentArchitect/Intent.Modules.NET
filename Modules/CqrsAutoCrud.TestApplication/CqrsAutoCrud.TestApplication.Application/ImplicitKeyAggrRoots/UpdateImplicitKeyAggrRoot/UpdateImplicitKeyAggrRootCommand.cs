using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRoot
{
    public class UpdateImplicitKeyAggrRootCommand : IRequest, ICommand
    {
        public string Attribute { get; set; }

        public Guid Id { get; set; }

        public List<UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO> ImplicitKeyNestedCompositions { get; set; }

    }
}