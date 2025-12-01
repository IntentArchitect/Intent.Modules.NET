using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Aws.Sqs.Api
{
    public static class EventingPackageModelStereotypeExtensions
    {
        public static AwsSqsPackageSettings GetAwsSqsPackageSettings(this EventingPackageModel model)
        {
            var stereotype = model.GetStereotype(AwsSqsPackageSettings.DefinitionId);
            return stereotype != null ? new AwsSqsPackageSettings(stereotype) : null;
        }


        public static bool HasAwsSqsPackageSettings(this EventingPackageModel model)
        {
            return model.HasStereotype(AwsSqsPackageSettings.DefinitionId);
        }

        public static bool TryGetAwsSqsPackageSettings(this EventingPackageModel model, out AwsSqsPackageSettings stereotype)
        {
            if (!HasAwsSqsPackageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AwsSqsPackageSettings(model.GetStereotype(AwsSqsPackageSettings.DefinitionId));
            return true;
        }

        public class AwsSqsPackageSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "6ffbabf9-435f-442b-866a-37c4a83bb770";

            public AwsSqsPackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}