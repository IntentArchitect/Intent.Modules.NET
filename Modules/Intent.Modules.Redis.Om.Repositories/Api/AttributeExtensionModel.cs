using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Redis.Om.Repositories.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AttributeExtensionModel : AttributeModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AttributeExtensionModel(IElement element) : base(element)
        {
        }

    }
}