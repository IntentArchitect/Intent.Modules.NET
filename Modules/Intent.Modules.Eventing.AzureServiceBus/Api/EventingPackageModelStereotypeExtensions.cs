using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.AzureServiceBus.Api
{
    public static class EventingPackageModelStereotypeExtensions
    {
        public static AzureServiceBusPackageSettings GetAzureServiceBusPackageSettings(this EventingPackageModel model)
        {
            var stereotype = model.GetStereotype(AzureServiceBusPackageSettings.DefinitionId);
            return stereotype != null ? new AzureServiceBusPackageSettings(stereotype) : null;
        }


        public static bool HasAzureServiceBusPackageSettings(this EventingPackageModel model)
        {
            return model.HasStereotype(AzureServiceBusPackageSettings.DefinitionId);
        }

        public static bool TryGetAzureServiceBusPackageSettings(this EventingPackageModel model, out AzureServiceBusPackageSettings stereotype)
        {
            if (!HasAzureServiceBusPackageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AzureServiceBusPackageSettings(model.GetStereotype(AzureServiceBusPackageSettings.DefinitionId));
            return true;
        }

        public class AzureServiceBusPackageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "7c7514b5-f17f-4c91-aade-c5b5b0634281";

            public AzureServiceBusPackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}