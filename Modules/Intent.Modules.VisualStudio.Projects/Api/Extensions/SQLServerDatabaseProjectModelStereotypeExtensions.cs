using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class SQLServerDatabaseProjectModelStereotypeExtensions
    {
        public static NETFrameworkSettings GetNETFrameworkSettings(this SQLServerDatabaseProjectModel model)
        {
            var stereotype = model.GetStereotype(NETFrameworkSettings.DefinitionId);
            return stereotype != null ? new NETFrameworkSettings(stereotype) : null;
        }


        public static bool HasNETFrameworkSettings(this SQLServerDatabaseProjectModel model)
        {
            return model.HasStereotype(NETFrameworkSettings.DefinitionId);
        }

        public static bool TryGetNETFrameworkSettings(this SQLServerDatabaseProjectModel model, out NETFrameworkSettings stereotype)
        {
            if (!HasNETFrameworkSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new NETFrameworkSettings(model.GetStereotype(NETFrameworkSettings.DefinitionId));
            return true;
        }

        public class NETFrameworkSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f3fcc35a-4bb2-44e7-aca2-d721e047a4c9";

            public NETFrameworkSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement TargetFramework()
            {
                return _stereotype.GetProperty<IElement>("Target Framework");
            }

            public class TargetFrameworkOptions
            {
                public readonly string Value;

                public TargetFrameworkOptions(string value)
                {
                    Value = value;
                }

                public TargetFrameworkOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "net452":
                            return TargetFrameworkOptionsEnum.Net452;
                        case "net462":
                            return TargetFrameworkOptionsEnum.Net462;
                        case "net472":
                            return TargetFrameworkOptionsEnum.Net472;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNet452()
                {
                    return Value == "net452";
                }
                public bool IsNet462()
                {
                    return Value == "net462";
                }
                public bool IsNet472()
                {
                    return Value == "net472";
                }
            }

            public enum TargetFrameworkOptionsEnum
            {
                Net452,
                Net462,
                Net472
            }
        }

    }
}