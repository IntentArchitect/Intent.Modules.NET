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
    public static class IntegrationCommandModelStereotypeExtensions
    {
        public static AwsSqs GetAwsSqs(this IntegrationCommandModel model)
        {
            var stereotype = model.GetStereotype(AwsSqs.DefinitionId);
            return stereotype != null ? new AwsSqs(stereotype) : null;
        }


        public static bool HasAwsSqs(this IntegrationCommandModel model)
        {
            return model.HasStereotype(AwsSqs.DefinitionId);
        }

        public static bool TryGetAwsSqs(this IntegrationCommandModel model, out AwsSqs stereotype)
        {
            if (!HasAwsSqs(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AwsSqs(model.GetStereotype(AwsSqs.DefinitionId));
            return true;
        }

        public class AwsSqs
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "74fbdee0-4098-4544-8ecf-f7c5787c78c3";

            public AwsSqs(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string QueueName()
            {
                return _stereotype.GetProperty<string>("Queue Name");
            }

        }

    }
}