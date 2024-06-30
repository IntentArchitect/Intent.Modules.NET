using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Eventing.Solace.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class FolderExtensionModel : FolderModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public FolderExtensionModel(IElement element) : base(element)
        {
        }

        public IList<QueueModel> Queues => _element.ChildElements
            .GetElementsOfType(QueueModel.SpecializationTypeId)
            .Select(x => new QueueModel(x))
            .ToList();

    }
}