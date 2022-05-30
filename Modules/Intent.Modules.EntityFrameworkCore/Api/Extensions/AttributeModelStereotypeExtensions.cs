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
        public static RowVersion GetRowVersion(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Row Version");
            return stereotype != null ? new RowVersion(stereotype) : null;
        }

        public static IReadOnlyCollection<RowVersion> GetRowVersions(this AttributeModel model)
        {
            var stereotypes = model
                .GetStereotypes("Row Version")
                .Select(stereotype => new RowVersion(stereotype))
                .ToArray();

            return stereotypes;
        }


        public static bool HasRowVersion(this AttributeModel model)
        {
            return model.HasStereotype("Row Version");
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