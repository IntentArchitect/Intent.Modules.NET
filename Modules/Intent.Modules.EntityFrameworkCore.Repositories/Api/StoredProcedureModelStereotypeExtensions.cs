using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Repositories.Api
{
    public static class StoredProcedureModelStereotypeExtensions
    {
        public static StoredProcedureSettings GetStoredProcedureSettings(this StoredProcedureModel model)
        {
            var stereotype = model.GetStereotype("Stored Procedure Settings");
            return stereotype != null ? new StoredProcedureSettings(stereotype) : null;
        }


        public static bool HasStoredProcedureSettings(this StoredProcedureModel model)
        {
            return model.HasStereotype("Stored Procedure Settings");
        }

        public static bool TryGetStoredProcedureSettings(this StoredProcedureModel model, out StoredProcedureSettings stereotype)
        {
            if (!HasStoredProcedureSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new StoredProcedureSettings(model.GetStereotype("Stored Procedure Settings"));
            return true;
        }

        public class StoredProcedureSettings
        {
            private IStereotype _stereotype;

            public StoredProcedureSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string NameInSchema()
            {
                return _stereotype.GetProperty<string>("Name in Schema");
            }

        }

    }
}