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
    public class EventingDTOExtensionModel : EventingDTOModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EventingDTOExtensionModel(IElement element) : base(element)
        {
        }

    }

    [IntentManaged(Mode.Fully)]
    public static class EventingDTOExtensionModelExtensions
    {

        public static bool HasMapFromDomainMapping(this EventingDTOModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "e437007c-33fd-46d5-9293-d4529b4b82e6";
        }

        public static IElementMapping GetMapFromDomainMapping(this EventingDTOModel type)
        {
            return type.HasMapFromDomainMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}