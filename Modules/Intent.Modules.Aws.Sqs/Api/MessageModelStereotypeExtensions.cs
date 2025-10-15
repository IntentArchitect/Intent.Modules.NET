using System;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Aws.Sqs.Api
{
    public static class MessageModelStereotypeExtensions
    {
        public static AwsSqs GetAwsSqs(this MessageModel model)
        {
            var stereotype = model.GetStereotype(AwsSqs.DefinitionId);
            return stereotype != null ? new AwsSqs(stereotype) : null;
        }

        public static bool HasAwsSqs(this MessageModel model)
        {
            return model.HasStereotype(AwsSqs.DefinitionId);
        }

        public static bool TryGetAwsSqs(this MessageModel model, out AwsSqs stereotype)
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
            public const string DefinitionId = "f0b7e50e-71a9-4f31-9f9a-3c3e0b5d8f2e";

            public AwsSqs(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string QueueName()
            {
                return _stereotype.GetProperty<string>("Queue Name");
            }

            public string QueueUrl()
            {
                return _stereotype.GetProperty<string>("Queue URL");
            }
        }
    }
}
