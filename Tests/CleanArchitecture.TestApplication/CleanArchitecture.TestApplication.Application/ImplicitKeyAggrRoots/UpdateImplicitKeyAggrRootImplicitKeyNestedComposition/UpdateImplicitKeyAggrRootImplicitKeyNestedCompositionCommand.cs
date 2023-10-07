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

        public Guid ImplicitKeyAggrRootId { get; private set; }
        public Guid Id { get; private set; }
        public string Attribute { get; set; }

        public void SetImplicitKeyAggrRootId(Guid implicitKeyAggrRootId)
        {
            if (ImplicitKeyAggrRootId == default)
            {
                ImplicitKeyAggrRootId = implicitKeyAggrRootId;
            }
        }

        public void SetId(Guid id)
        {
            if (Id == default)
            {
                Id = id;
            }
        }
    }
}