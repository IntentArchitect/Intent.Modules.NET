using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Eventing.Contracts.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MessageExtensionModel : MessageModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageExtensionModel(IElement element) : base(element)
        {
        }

    }
}