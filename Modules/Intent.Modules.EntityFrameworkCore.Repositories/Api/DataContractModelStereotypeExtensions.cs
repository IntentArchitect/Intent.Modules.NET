using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Repositories.Api
{
    public static class DataContractModelStereotypeExtensions
    {
        public static UserDefinedTableTypeSettings GetUserDefinedTableTypeSettings(this DataContractModel model)
        {
            var stereotype = model.GetStereotype("User-Defined Table Type Settings");
            return stereotype != null ? new UserDefinedTableTypeSettings(stereotype) : null;
        }


        public static bool HasUserDefinedTableTypeSettings(this DataContractModel model)
        {
            return model.HasStereotype("User-Defined Table Type Settings");
        }

        public static bool TryGetUserDefinedTableTypeSettings(this DataContractModel model, out UserDefinedTableTypeSettings stereotype)
        {
            if (!HasUserDefinedTableTypeSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new UserDefinedTableTypeSettings(model.GetStereotype("User-Defined Table Type Settings"));
            return true;
        }

        public class UserDefinedTableTypeSettings
        {
            private IStereotype _stereotype;

            public UserDefinedTableTypeSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

        }

    }
}