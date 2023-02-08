using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.CreateImplicitKeyAggrRootImplicitKeyNestedComposition
{
    public class CreateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand : IRequest<Guid>, ICommand
    {
        public Guid ImplicitKeyAggrRootId { get; set; }

        public string Attribute { get; set; }

    }
}