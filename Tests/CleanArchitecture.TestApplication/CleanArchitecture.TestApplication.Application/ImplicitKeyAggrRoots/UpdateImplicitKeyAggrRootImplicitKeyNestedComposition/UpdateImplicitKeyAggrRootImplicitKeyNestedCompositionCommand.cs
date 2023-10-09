using System;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.ImplicitKeyAggrRoots.UpdateImplicitKeyAggrRootImplicitKeyNestedComposition
{
    public class UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand : IRequest, ICommand
    {
        public UpdateImplicitKeyAggrRootImplicitKeyNestedCompositionCommand(Guid implicitKeyAggrRootId,
            Guid id,
            string attribute)
        {
            ImplicitKeyAggrRootId = implicitKeyAggrRootId;
            Id = id;
            Attribute = attribute;
        }

        public Guid ImplicitKeyAggrRootId { get; set; }
        public Guid Id { get; set; }
        public string Attribute { get; set; }
    }
}