using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Eventing.MassTransit.Api
{
    public static class EventingPackageModelStereotypeExtensions
    {
        public static MassTransitPackageSettings GetMassTransitPackageSettings(this EventingPackageModel model)
        {
            var stereotype = model.GetStereotype(MassTransitPackageSettings.DefinitionId);
            return stereotype != null ? new MassTransitPackageSettings(stereotype) : null;
        }


        public static bool HasMassTransitPackageSettings(this EventingPackageModel model)
        {
            return model.HasStereotype(MassTransitPackageSettings.DefinitionId);
        }

        public static bool TryGetMassTransitPackageSettings(this EventingPackageModel model, out MassTransitPackageSettings stereotype)
        {
            if (!HasMassTransitPackageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MassTransitPackageSettings(model.GetStereotype(MassTransitPackageSettings.DefinitionId));
            return true;
        }

        public class MassTransitPackageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f997aa34-0a9a-4733-b5a9-fdcf9e37170c";

            public MassTransitPackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}