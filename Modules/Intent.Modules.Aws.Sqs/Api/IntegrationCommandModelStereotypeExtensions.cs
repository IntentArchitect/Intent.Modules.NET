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
        public static AWSSQS GetAWSSQS(this IntegrationCommandModel model)
        {
            var stereotype = model.GetStereotype(AWSSQS.DefinitionId);
            return stereotype != null ? new AWSSQS(stereotype) : null;
        }


        public static bool HasAWSSQS(this IntegrationCommandModel model)
        {
            return model.HasStereotype(AWSSQS.DefinitionId);
        }

        public static bool TryGetAWSSQS(this IntegrationCommandModel model, out AWSSQS stereotype)
        {
            if (!HasAWSSQS(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new AWSSQS(model.GetStereotype(AWSSQS.DefinitionId));
            return true;
        }

        public class AWSSQS
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "74fbdee0-4098-4544-8ecf-f7c5787c78c3";

            public AWSSQS(IStereotype stereotype)
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