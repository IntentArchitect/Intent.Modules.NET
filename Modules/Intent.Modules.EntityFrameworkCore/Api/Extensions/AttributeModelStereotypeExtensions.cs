using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Api
{
    public static class AttributeModelStereotypeExtensions
    {
        public static PartitionKey GetPartitionKey(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("3a203a3e-116e-4a7c-b375-e690570efc3f");
            return stereotype != null ? new PartitionKey(stereotype) : null;
        }


        public static bool HasPartitionKey(this AttributeModel model)
        {
            return model.HasStereotype("3a203a3e-116e-4a7c-b375-e690570efc3f");
        }

        public static bool TryGetPartitionKey(this AttributeModel model, out PartitionKey stereotype)
        {
            if (!HasPartitionKey(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PartitionKey(model.GetStereotype("3a203a3e-116e-4a7c-b375-e690570efc3f"));
            return true;
        }
        public static RowVersion GetRowVersion(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("41adb04f-ad7d-4969-8954-265e1ba8a783");
            return stereotype != null ? new RowVersion(stereotype) : null;
        }


        public static bool HasRowVersion(this AttributeModel model)
        {
            return model.HasStereotype("41adb04f-ad7d-4969-8954-265e1ba8a783");
        }

        public static bool TryGetRowVersion(this AttributeModel model, out RowVersion stereotype)
        {
            if (!HasRowVersion(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RowVersion(model.GetStereotype("41adb04f-ad7d-4969-8954-265e1ba8a783"));
            return true;
        }

        public class PartitionKey
        {
            private IStereotype _stereotype;

            public PartitionKey(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }


        public class RowVersion
        {
            private IStereotype _stereotype;

            public RowVersion(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}