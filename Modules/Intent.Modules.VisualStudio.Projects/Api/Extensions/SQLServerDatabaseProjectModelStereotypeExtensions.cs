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

        public static SQLServerDatabaseProject GetSQLServerDatabaseProject(this SQLServerDatabaseProjectModel model)
        {
            var stereotype = model.GetStereotype(SQLServerDatabaseProject.DefinitionId);
            return stereotype != null ? new SQLServerDatabaseProject(stereotype) : null;
        }


        public static bool HasSQLServerDatabaseProject(this SQLServerDatabaseProjectModel model)
        {
            return model.HasStereotype(SQLServerDatabaseProject.DefinitionId);
        }

        public static bool TryGetSQLServerDatabaseProject(this SQLServerDatabaseProjectModel model, out SQLServerDatabaseProject stereotype)
        {
            if (!HasSQLServerDatabaseProject(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SQLServerDatabaseProject(model.GetStereotype(SQLServerDatabaseProject.DefinitionId));
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
        }

        public class SQLServerDatabaseProject
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "b8201720-882a-42ad-abb6-870a97815334";

            public SQLServerDatabaseProject(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public ProjectTypeOptions ProjectType()
            {
                return new ProjectTypeOptions(_stereotype.GetProperty<string>("Project Type"));
            }

            public string Version()
            {
                return _stereotype.GetProperty<string>("Version");
            }

            public class ProjectTypeOptions
            {
                public readonly string Value;

                public ProjectTypeOptions(string value)
                {
                    Value = value;
                }

                public ProjectTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case ".NET Framework":
                            return ProjectTypeOptionsEnum.NETFramework;
                        case "SDK":
                            return ProjectTypeOptionsEnum.SDK;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNETFramework()
                {
                    return Value == ".NET Framework";
                }
                public bool IsSDK()
                {
                    return Value == "SDK";
                }
            }

            public enum ProjectTypeOptionsEnum
            {
                NETFramework,
                SDK
            }
        }

    }
}