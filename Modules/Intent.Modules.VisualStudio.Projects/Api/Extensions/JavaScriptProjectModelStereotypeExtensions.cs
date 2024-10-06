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
    public static class JavaScriptProjectModelStereotypeExtensions
    {
        public static JavaScriptProjectOptions GetJavaScriptProjectOptions(this JavaScriptProjectModel model)
        {
            var stereotype = model.GetStereotype(JavaScriptProjectOptions.DefinitionId);
            return stereotype != null ? new JavaScriptProjectOptions(stereotype) : null;
        }


        public static bool HasJavaScriptProjectOptions(this JavaScriptProjectModel model)
        {
            return model.HasStereotype(JavaScriptProjectOptions.DefinitionId);
        }

        public static bool TryGetJavaScriptProjectOptions(this JavaScriptProjectModel model, out JavaScriptProjectOptions stereotype)
        {
            if (!HasJavaScriptProjectOptions(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new JavaScriptProjectOptions(model.GetStereotype(JavaScriptProjectOptions.DefinitionId));
            return true;
        }
        public static JavaScriptSettings GetJavaScriptSettings(this JavaScriptProjectModel model)
        {
            var stereotype = model.GetStereotype(JavaScriptSettings.DefinitionId);
            return stereotype != null ? new JavaScriptSettings(stereotype) : null;
        }


        public static bool HasJavaScriptSettings(this JavaScriptProjectModel model)
        {
            return model.HasStereotype(JavaScriptSettings.DefinitionId);
        }

        public static bool TryGetJavaScriptSettings(this JavaScriptProjectModel model, out JavaScriptSettings stereotype)
        {
            if (!HasJavaScriptSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new JavaScriptSettings(model.GetStereotype(JavaScriptSettings.DefinitionId));
            return true;
        }

        public class JavaScriptProjectOptions
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8ec9ad63-9d46-4d3d-954f-2adb7d9f18a2";

            public JavaScriptProjectOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string RelativeLocation()
            {
                return _stereotype.GetProperty<string>("Relative Location");
            }

        }

        public class JavaScriptSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "141b4305-433b-4d5a-97ed-7796eabbe2aa";

            public JavaScriptSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public ShouldRunNpmInstallOptions ShouldRunNpmInstall()
            {
                return new ShouldRunNpmInstallOptions(_stereotype.GetProperty<string>("Should run npm install"));
            }

            public ShouldRunBuildScriptOptions ShouldRunBuildScript()
            {
                return new ShouldRunBuildScriptOptions(_stereotype.GetProperty<string>("Should run build script"));
            }

            public string BuildCommand()
            {
                return _stereotype.GetProperty<string>("Build command");
            }

            public string StartupCommand()
            {
                return _stereotype.GetProperty<string>("Startup command");
            }

            public string TestCommand()
            {
                return _stereotype.GetProperty<string>("Test command");
            }

            public string CleanCommand()
            {
                return _stereotype.GetProperty<string>("Clean command");
            }

            public string PublishCommand()
            {
                return _stereotype.GetProperty<string>("Publish command");
            }

            public class ShouldRunNpmInstallOptions
            {
                public readonly string Value;

                public ShouldRunNpmInstallOptions(string value)
                {
                    Value = value;
                }

                public ShouldRunNpmInstallOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "(unspecified)":
                            return ShouldRunNpmInstallOptionsEnum.Unspecified;
                        case "false":
                            return ShouldRunNpmInstallOptionsEnum.False;
                        case "true":
                            return ShouldRunNpmInstallOptionsEnum.True;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsUnspecified()
                {
                    return Value == "(unspecified)";
                }
                public bool IsFalse()
                {
                    return Value == "false";
                }
                public bool IsTrue()
                {
                    return Value == "true";
                }
            }

            public enum ShouldRunNpmInstallOptionsEnum
            {
                Unspecified,
                False,
                True
            }
            public class ShouldRunBuildScriptOptions
            {
                public readonly string Value;

                public ShouldRunBuildScriptOptions(string value)
                {
                    Value = value;
                }

                public ShouldRunBuildScriptOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "(unspecified)":
                            return ShouldRunBuildScriptOptionsEnum.Unspecified;
                        case "false":
                            return ShouldRunBuildScriptOptionsEnum.False;
                        case "true":
                            return ShouldRunBuildScriptOptionsEnum.True;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsUnspecified()
                {
                    return Value == "(unspecified)";
                }
                public bool IsFalse()
                {
                    return Value == "false";
                }
                public bool IsTrue()
                {
                    return Value == "true";
                }
            }

            public enum ShouldRunBuildScriptOptionsEnum
            {
                Unspecified,
                False,
                True
            }
        }

    }
}