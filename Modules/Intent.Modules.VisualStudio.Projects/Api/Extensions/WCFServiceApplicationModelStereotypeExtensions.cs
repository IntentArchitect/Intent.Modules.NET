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
    public static class WCFServiceApplicationModelStereotypeExtensions
    {
        public static NETFrameworkSettings GetNETFrameworkSettings(this WCFServiceApplicationModel model)
        {
            var stereotype = model.GetStereotype(NETFrameworkSettings.DefinitionId);
            return stereotype != null ? new NETFrameworkSettings(stereotype) : null;
        }


        public static bool HasNETFrameworkSettings(this WCFServiceApplicationModel model)
        {
            return model.HasStereotype(NETFrameworkSettings.DefinitionId);
        }

        public static bool TryGetNETFrameworkSettings(this WCFServiceApplicationModel model, out NETFrameworkSettings stereotype)
        {
            if (!HasNETFrameworkSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new NETFrameworkSettings(model.GetStereotype(NETFrameworkSettings.DefinitionId));
            return true;
        }

        public static CSharpProjectOptions GetCSharpProjectOptions(this WCFServiceApplicationModel model)
        {
            var stereotype = model.GetStereotype(CSharpProjectOptions.DefinitionId);
            return stereotype != null ? new CSharpProjectOptions(stereotype) : null;
        }


        public static bool HasCSharpProjectOptions(this WCFServiceApplicationModel model)
        {
            return model.HasStereotype(CSharpProjectOptions.DefinitionId);
        }

        public static bool TryGetCSharpProjectOptions(this WCFServiceApplicationModel model, out CSharpProjectOptions stereotype)
        {
            if (!HasCSharpProjectOptions(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CSharpProjectOptions(model.GetStereotype(CSharpProjectOptions.DefinitionId));
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

        public class CSharpProjectOptions
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "d9b43864-0370-401b-9e71-b1d286919500";

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

            public NullableOptions Nullable()
            {
                return new NullableOptions(_stereotype.GetProperty<string>("Nullable"));
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

                public LanguageVersionOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "default":
                            return LanguageVersionOptionsEnum.Default;
                        case "latest":
                            return LanguageVersionOptionsEnum.Latest;
                        case "preview":
                            return LanguageVersionOptionsEnum.Preview;
                        case "12.0":
                            return LanguageVersionOptionsEnum._120;
                        case "11.0":
                            return LanguageVersionOptionsEnum._110;
                        case "10.0":
                            return LanguageVersionOptionsEnum._100;
                        case "9.0":
                            return LanguageVersionOptionsEnum._90;
                        case "8.0":
                            return LanguageVersionOptionsEnum._80;
                        case "7.3":
                            return LanguageVersionOptionsEnum._73;
                        case "7.2":
                            return LanguageVersionOptionsEnum._72;
                        case "7.1":
                            return LanguageVersionOptionsEnum._71;
                        case "7":
                            return LanguageVersionOptionsEnum._7;
                        case "6":
                            return LanguageVersionOptionsEnum._6;
                        case "5":
                            return LanguageVersionOptionsEnum._5;
                        case "4":
                            return LanguageVersionOptionsEnum._4;
                        case "3":
                            return LanguageVersionOptionsEnum._3;
                        case "2":
                            return LanguageVersionOptionsEnum._2;
                        case "1":
                            return LanguageVersionOptionsEnum._1;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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
                public bool Is120()
                {
                    return Value == "12.0";
                }
                public bool Is110()
                {
                    return Value == "11.0";
                }
                public bool Is100()
                {
                    return Value == "10.0";
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

            public enum LanguageVersionOptionsEnum
            {
                Default,
                Latest,
                Preview,
                _120,
                _110,
                _100,
                _90,
                _80,
                _73,
                _72,
                _71,
                _7,
                _6,
                _5,
                _4,
                _3,
                _2,
                _1
            }
            public class NullableOptions
            {
                public readonly string Value;

                public NullableOptions(string value)
                {
                    Value = value;
                }

                public NullableOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "(unspecified)":
                            return NullableOptionsEnum.Unspecified;
                        case "disable":
                            return NullableOptionsEnum.Disable;
                        case "enable":
                            return NullableOptionsEnum.Enable;
                        case "warnings":
                            return NullableOptionsEnum.Warnings;
                        case "annotations":
                            return NullableOptionsEnum.Annotations;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsUnspecified()
                {
                    return Value == "(unspecified)";
                }

                public bool IsDisable()
                {
                    return Value == "disable";
                }
                public bool IsEnable()
                {
                    return Value == "enable";
                }
                public bool IsWarnings()
                {
                    return Value == "warnings";
                }
                public bool IsAnnotations()
                {
                    return Value == "annotations";
                }
            }

            public enum NullableOptionsEnum
            {
                Unspecified,
                Disable,
                Enable,
                Warnings,
                Annotations
            }
        }

    }
}