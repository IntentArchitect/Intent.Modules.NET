using System;
using System.Collections.Generic;
using CqrsAutoCrud.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CqrsAutoCrud.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRoot
{
    public class CreateImplicitKeyAggrRootCommand : IRequest<Guid>, ICommand
    {
        public string Attribute { get; set; }

        public List<CreateImplicitKeyAggrRootImplicitKeyNestedCompositionDTO> ImplicitKeyNestedCompositions { get; set; }

    }
}