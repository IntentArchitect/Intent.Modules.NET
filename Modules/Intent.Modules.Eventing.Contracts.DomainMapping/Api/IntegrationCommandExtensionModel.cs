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
    public class IntegrationCommandExtensionModel : IntegrationCommandModel
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationCommandExtensionModel(IElement element) : base(element)
        {
        }

    }

    [IntentManaged(Mode.Fully)]
    public static class IntegrationCommandExtensionModelExtensions
    {

        public static bool HasMapFromDomainMapping(this IntegrationCommandModel type)
        {
            return type.InternalElement.MappedElement?.MappingSettingsId == "e72001cc-e117-4919-9a7b-bd8d8633f8d7";
        }

        public static IElementMapping GetMapFromDomainMapping(this IntegrationCommandModel type)
        {
            return type.HasMapFromDomainMapping() ? type.InternalElement.MappedElement : null;
        }
    }
}