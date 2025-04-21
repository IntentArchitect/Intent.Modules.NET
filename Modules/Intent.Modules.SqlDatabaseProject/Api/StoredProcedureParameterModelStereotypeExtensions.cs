using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.SqlDatabaseProject.Api
{
    public static class StoredProcedureParameterModelStereotypeExtensions
    {
        public static StoredProcedureParameterSettings GetStoredProcedureParameterSettings(this StoredProcedureParameterModel model)
        {
            var stereotype = model.GetStereotype(StoredProcedureParameterSettings.DefinitionId);
            return stereotype != null ? new StoredProcedureParameterSettings(stereotype) : null;
        }


        public static bool HasStoredProcedureParameterSettings(this StoredProcedureParameterModel model)
        {
            return model.HasStereotype(StoredProcedureParameterSettings.DefinitionId);
        }

        public static bool TryGetStoredProcedureParameterSettings(this StoredProcedureParameterModel model, out StoredProcedureParameterSettings stereotype)
        {
            if (!HasStoredProcedureParameterSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new StoredProcedureParameterSettings(model.GetStereotype(StoredProcedureParameterSettings.DefinitionId));
            return true;
        }

        public class StoredProcedureParameterSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "517cdcf9-5c8d-443f-99e5-d32095d26a10";

            public StoredProcedureParameterSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string SQLDataType()
            {
                return _stereotype.GetProperty<string>("SQL Data Type");
            }

        }

    }
}