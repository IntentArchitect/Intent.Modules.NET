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
            var stereotype = model.GetStereotype("5332b774-6499-4b4b-9fdb-e3eef13bdee4");
            return stereotype != null ? new StoredProcedureParameterSettings(stereotype) : null;
        }


        public static bool HasStoredProcedureParameterSettings(this StoredProcedureParameterModel model)
        {
            return model.HasStereotype("5332b774-6499-4b4b-9fdb-e3eef13bdee4");
        }

        public static bool TryGetStoredProcedureParameterSettings(this StoredProcedureParameterModel model, out StoredProcedureParameterSettings stereotype)
        {
            if (!HasStoredProcedureParameterSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new StoredProcedureParameterSettings(model.GetStereotype("5332b774-6499-4b4b-9fdb-e3eef13bdee4"));
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

            public SQLStringTypeOptions SQLStringType()
            {
                return new SQLStringTypeOptions(_stereotype.GetProperty<string>("SQL String Type"));
            }

            public int? Size()
            {
                return _stereotype.GetProperty<int?>("Size");
            }

            public int? Precision()
            {
                return _stereotype.GetProperty<int?>("Precision");
            }

            public int? Scale()
            {
                return _stereotype.GetProperty<int?>("Scale");
            }

            public class SQLStringTypeOptions
            {
                public readonly string Value;

                public SQLStringTypeOptions(string value)
                {
                    Value = value;
                }

                public SQLStringTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "VarChar":
                            return SQLStringTypeOptionsEnum.VarChar;
                        case "NVarChar":
                            return SQLStringTypeOptionsEnum.NVarChar;
                        case "Char":
                            return SQLStringTypeOptionsEnum.Char;
                        case "NChar":
                            return SQLStringTypeOptionsEnum.NChar;
                        case "Text":
                            return SQLStringTypeOptionsEnum.Text;
                        case "NText":
                            return SQLStringTypeOptionsEnum.NText;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsVarChar()
                {
                    return Value == "VarChar";
                }
                public bool IsNVarChar()
                {
                    return Value == "NVarChar";
                }
                public bool IsChar()
                {
                    return Value == "Char";
                }
                public bool IsNChar()
                {
                    return Value == "NChar";
                }
                public bool IsText()
                {
                    return Value == "Text";
                }
                public bool IsNText()
                {
                    return Value == "NText";
                }
            }

            public enum SQLStringTypeOptionsEnum
            {
                VarChar,
                NVarChar,
                Char,
                NChar,
                Text,
                NText
            }

        }

    }
}