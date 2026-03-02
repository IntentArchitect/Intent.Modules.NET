using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Eventing.Contracts.DomainMapping.Api
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
            return type.InternalElement.MappedElement?.MappingSettingsId == "1b4f670b-4c97-4dd0-a1c5-831f3a695859";
        }

        public static IElementMapping GetMapFromDomainMapping(this MessageModel type)
        {
            return type.HasMapFromDomainMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}