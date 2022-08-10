using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.CosmosDb.Api
{
    public static class AttributeModelStereotypeExtensions
    {
        public static PartitionKey GetPartitionKey(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Partition Key");
            return stereotype != null ? new PartitionKey(stereotype) : null;
        }


        public static bool HasPartitionKey(this AttributeModel model)
        {
            return model.HasStereotype("Partition Key");
        }

        public static bool TryGetPartitionKey(this AttributeModel model, out PartitionKey stereotype)
        {
            if (!HasPartitionKey(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PartitionKey(model.GetStereotype("Partition Key"));
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

    }
}