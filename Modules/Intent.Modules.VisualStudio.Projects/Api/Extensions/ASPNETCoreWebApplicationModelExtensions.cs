using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class ASPNETCoreWebApplicationModelExtensions
    {
        public static NETCoreSettings GetNETCoreSettings(this ASPNETCoreWebApplicationModel model)
        {
            var stereotype = model.GetStereotype(".NET Core Settings");
            return stereotype != null ? new NETCoreSettings(stereotype) : null;
        }

        public static bool HasNETCoreSettings(this ASPNETCoreWebApplicationModel model)
        {
            return model.HasStereotype(".NET Core Settings");
        }

        public static CSharpProjectOptions GetCSharpProjectOptions(this ASPNETCoreWebApplicationModel model)
        {
            var stereotype = model.GetStereotype("C# Project Options");
            return stereotype != null ? new CSharpProjectOptions(stereotype) : null;
        }

        public static bool HasCSharpProjectOptions(this ASPNETCoreWebApplicationModel model)
        {
            return model.HasStereotype("C# Project Options");
        }


        public class NETCoreSettings
        {
            private IStereotype _stereotype;

            public NETCoreSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool TargetMultipleFrameworks()
            {
                return _stereotype.GetProperty<bool>("Target Multiple Frameworks");
            }

            public IElement TargetFramework()
            {
                return _stereotype.GetProperty<IElement>("Target Framework");
            }

            public IElement[] TargetFrameworks()
            {
                return _stereotype.GetProperty<IElement[]>("Target Frameworks");
            }

        }

        public class CSharpProjectOptions
        {
            private IStereotype _stereotype;

            public CSharpProjectOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public LanguageVersionOptions LanguageVersion()
            {
                return new LanguageVersionOptions(_stereotype.GetProperty<string>("Language Version"));
            }

            public string RelativeLocation()
            {
                return _stereotype.GetProperty<string>("Relative Location");
            }

            public bool NullableEnabled()
            {
                return _stereotype.GetProperty<bool>("Nullable Enabled");
            }

            public class LanguageVersionOptions
            {
                public readonly string Value;

                public LanguageVersionOptions(string value)
                {
                    Value = value;
                }

                public bool IsDefault()
                {
                    return Value == "default";
                }
                public bool IsLatest()
                {
                    return Value == "latest";
                }
                public bool IsPreview()
                {
                    return Value == "preview";
                }
                public bool Is90()
                {
                    return Value == "9.0";
                }
                public bool Is80()
                {
                    return Value == "8.0";
                }
                public bool Is73()
                {
                    return Value == "7.3";
                }
                public bool Is72()
                {
                    return Value == "7.2";
                }
                public bool Is71()
                {
                    return Value == "7.1";
                }
                public bool Is7()
                {
                    return Value == "7";
                }
                public bool Is6()
                {
                    return Value == "6";
                }
                public bool Is5()
                {
                    return Value == "5";
                }
                public bool Is4()
                {
                    return Value == "4";
                }
                public bool Is3()
                {
                    return Value == "3";
                }
                public bool Is2()
                {
                    return Value == "2";
                }
                public bool Is1()
                {
                    return Value == "1";
                }
            }

        }

    }
}