using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Repositories.Api
{
    public static class StoredProcedureParameterModelStereotypeExtensions
    {
        public static StoredProcedureParameterSettings GetStoredProcedureParameterSettings(this StoredProcedureParameterModel model)
        {
            var stereotype = model.GetStereotype("Stored Procedure Parameter Settings");
            return stereotype != null ? new StoredProcedureParameterSettings(stereotype) : null;
        }


        public static bool HasStoredProcedureParameterSettings(this StoredProcedureParameterModel model)
        {
            return model.HasStereotype("Stored Procedure Parameter Settings");
        }

        public static bool TryGetStoredProcedureParameterSettings(this StoredProcedureParameterModel model, out StoredProcedureParameterSettings stereotype)
        {
            if (!HasStoredProcedureParameterSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new StoredProcedureParameterSettings(model.GetStereotype("Stored Procedure Parameter Settings"));
            return true;
        }

        public class StoredProcedureParameterSettings
        {
            private IStereotype _stereotype;

            public StoredProcedureParameterSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool IsOutputParameter()
            {
                return _stereotype.GetProperty<bool>("Is Output Parameter");
            }

        }

    }
}