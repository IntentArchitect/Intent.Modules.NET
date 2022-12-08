using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.CRUD.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class MessageExtensionModel : MessageModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessageExtensionModel(IElement element) : base(element)
        {
        }

    }

    [IntentManaged(Mode.Fully)]
    public static class MessageExtensionModelExtensions
    {

        public static bool HasMapFromDomainMapping(this MessageModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "90d1d215-9f2c-4cec-99b4-3f3ceb75f874";
        }

        public static IElementMapping GetMapFromDomainMapping(this MessageModel type)
        {
            return type.HasMapFromDomainMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}