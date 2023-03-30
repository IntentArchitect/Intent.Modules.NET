using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.MongoDb.DomainEvents.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<DomainEventModel> DomainEvents => _element.ChildElements
            .GetElementsOfType(DomainEventModel.SpecializationTypeId)
            .Select(x => new DomainEventModel(x))
            .ToList();

    }
}