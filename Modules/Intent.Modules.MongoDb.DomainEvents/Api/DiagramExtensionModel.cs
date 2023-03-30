using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.MongoDb.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.MongoDb.DomainEvents.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DiagramExtensionModel : MongoDBDiagramModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DiagramExtensionModel(IElement element) : base(element)
        {
        }

    }
}