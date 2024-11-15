using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Api
{
    // This is for disambiguating the extension method
    using Intent.Modelers.Domain.Api;

    public static class AttributeModelStereotypeExtensions
    {
        public static PartitionKey GetPartitionKey(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(PartitionKey.DefinitionId);
            return stereotype != null ? new PartitionKey(stereotype) : null;
        }


        public static bool HasPartitionKey(this AttributeModel model)
        {
            return model.HasStereotype(PartitionKey.DefinitionId);
        }

        public static bool TryGetPartitionKey(this AttributeModel model, out PartitionKey stereotype)
        {
            if (!HasPartitionKey(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PartitionKey(model.GetStereotype(PartitionKey.DefinitionId));
            return true;
        }
        public static RowVersion GetRowVersion(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(RowVersion.DefinitionId);
            return stereotype != null ? new RowVersion(stereotype) : null;
        }


        public static bool HasRowVersion(this AttributeModel model)
        {
            return model.HasStereotype(RowVersion.DefinitionId);
        }

        public static bool TryGetRowVersion(this AttributeModel model, out RowVersion stereotype)
        {
            if (!HasRowVersion(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RowVersion(model.GetStereotype(RowVersion.DefinitionId));
            return true;
        }

        public class PartitionKey
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "3a203a3e-116e-4a7c-b375-e690570efc3f";

            public PartitionKey(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }


        public class RowVersion
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "41adb04f-ad7d-4969-8954-265e1ba8a783";

            public RowVersion(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}