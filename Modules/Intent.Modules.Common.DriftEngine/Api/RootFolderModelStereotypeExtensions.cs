using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Common.DriftEngine.Api
{
    public static class RootFolderModelStereotypeExtensions
    {
        public static DriftCheck GetDriftCheck(this RootFolderModel model)
        {
            var stereotype = model.GetStereotype(DriftCheck.DefinitionId);
            return stereotype != null ? new DriftCheck(stereotype) : null;
        }


        public static bool HasDriftCheck(this RootFolderModel model)
        {
            return model.HasStereotype(DriftCheck.DefinitionId);
        }

        public static bool TryGetDriftCheck(this RootFolderModel model, out DriftCheck stereotype)
        {
            if (!HasDriftCheck(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DriftCheck(model.GetStereotype(DriftCheck.DefinitionId));
            return true;
        }

        public static DriftCheckException GetDriftCheckException(this RootFolderModel model)
        {
            var stereotype = model.GetStereotype(DriftCheckException.DefinitionId);
            return stereotype != null ? new DriftCheckException(stereotype) : null;
        }


        public static bool HasDriftCheckException(this RootFolderModel model)
        {
            return model.HasStereotype(DriftCheckException.DefinitionId);
        }

        public static bool TryGetDriftCheckException(this RootFolderModel model, out DriftCheckException stereotype)
        {
            if (!HasDriftCheckException(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DriftCheckException(model.GetStereotype(DriftCheckException.DefinitionId));
            return true;
        }

        public static DriftCheckOptInRule GetDriftCheckOptInRule(this RootFolderModel model)
        {
            var stereotype = model.GetStereotype(DriftCheckOptInRule.DefinitionId);
            return stereotype != null ? new DriftCheckOptInRule(stereotype) : null;
        }


        public static bool HasDriftCheckOptInRule(this RootFolderModel model)
        {
            return model.HasStereotype(DriftCheckOptInRule.DefinitionId);
        }

        public static bool TryGetDriftCheckOptInRule(this RootFolderModel model, out DriftCheckOptInRule stereotype)
        {
            if (!HasDriftCheckOptInRule(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DriftCheckOptInRule(model.GetStereotype(DriftCheckOptInRule.DefinitionId));
            return true;
        }

        public static DriftCheckSeverityOverride GetDriftCheckSeverityOverride(this RootFolderModel model)
        {
            var stereotype = model.GetStereotype(DriftCheckSeverityOverride.DefinitionId);
            return stereotype != null ? new DriftCheckSeverityOverride(stereotype) : null;
        }


        public static bool HasDriftCheckSeverityOverride(this RootFolderModel model)
        {
            return model.HasStereotype(DriftCheckSeverityOverride.DefinitionId);
        }

        public static bool TryGetDriftCheckSeverityOverride(this RootFolderModel model, out DriftCheckSeverityOverride stereotype)
        {
            if (!HasDriftCheckSeverityOverride(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DriftCheckSeverityOverride(model.GetStereotype(DriftCheckSeverityOverride.DefinitionId));
            return true;
        }

        public static DriftScope GetDriftScope(this RootFolderModel model)
        {
            var stereotype = model.GetStereotype(DriftScope.DefinitionId);
            return stereotype != null ? new DriftScope(stereotype) : null;
        }


        public static bool HasDriftScope(this RootFolderModel model)
        {
            return model.HasStereotype(DriftScope.DefinitionId);
        }

        public static bool TryGetDriftScope(this RootFolderModel model, out DriftScope stereotype)
        {
            if (!HasDriftScope(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DriftScope(model.GetStereotype(DriftScope.DefinitionId));
            return true;
        }

        public static DriftScopeEntry GetDriftScopeEntry(this RootFolderModel model)
        {
            var stereotype = model.GetStereotype(DriftScopeEntry.DefinitionId);
            return stereotype != null ? new DriftScopeEntry(stereotype) : null;
        }


        public static bool HasDriftScopeEntry(this RootFolderModel model)
        {
            return model.HasStereotype(DriftScopeEntry.DefinitionId);
        }

        public static bool TryGetDriftScopeEntry(this RootFolderModel model, out DriftScopeEntry stereotype)
        {
            if (!HasDriftScopeEntry(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DriftScopeEntry(model.GetStereotype(DriftScopeEntry.DefinitionId));
            return true;
        }

        public class DriftCheck
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "baf16c68-1491-4c76-97ad-be1f3f4661ce";

            public DriftCheck(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IStereotype[] OptInRules()
            {
                return _stereotype.GetProperty<IStereotype[]>("Opt-in Rules") ?? new IStereotype[0];
            }
            public IStereotype[] Exceptions()
            {
                return _stereotype.GetProperty<IStereotype[]>("Exceptions") ?? new IStereotype[0];
            }
            public IStereotype[] SeverityOverrides()
            {
                return _stereotype.GetProperty<IStereotype[]>("Severity Overrides") ?? new IStereotype[0];
            }
        }

        public class DriftCheckException
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "890d0cac-4620-452c-a5de-8d569c367e8e";

            public DriftCheckException(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Rule()
            {
                return _stereotype.GetProperty<string>("Rule");
            }

            public string Match()
            {
                return _stereotype.GetProperty<string>("Match");
            }

            public string Reason()
            {
                return _stereotype.GetProperty<string>("Reason");
            }

        }

        public class DriftCheckOptInRule
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8648e3bd-177b-4fc4-908d-96cc4ccd2009";

            public DriftCheckOptInRule(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Rule()
            {
                return _stereotype.GetProperty<string>("Rule");
            }

            public string Glob()
            {
                return _stereotype.GetProperty<string>("Glob");
            }

        }

        public class DriftCheckSeverityOverride
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "923a39c2-7398-4f5a-b183-a9623694df82";

            public DriftCheckSeverityOverride(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Rule()
            {
                return _stereotype.GetProperty<string>("Rule");
            }

            public SeverityOptions Severity()
            {
                return new SeverityOptions(_stereotype.GetProperty<string>("Severity"));
            }

            public string Reason()
            {
                return _stereotype.GetProperty<string>("Reason");
            }

            public class SeverityOptions
            {
                public readonly string Value;

                public SeverityOptions(string value)
                {
                    Value = value;
                }

                public SeverityOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Error":
                            return SeverityOptionsEnum.Error;
                        case "Warning":
                            return SeverityOptionsEnum.Warning;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsError()
                {
                    return Value == "Error";
                }
                public bool IsWarning()
                {
                    return Value == "Warning";
                }
            }

            public enum SeverityOptionsEnum
            {
                Error,
                Warning
            }
        }

        public class DriftScope
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "857eba81-5138-429c-a97c-2f4b613e8d8c";

            public DriftScope(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IStereotype[] Entries()
            {
                return _stereotype.GetProperty<IStereotype[]>("Entries") ?? new IStereotype[0];
            }
        }

        public class DriftScopeEntry
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "fe20f2d9-4d95-44c8-96cb-9b129d9e56a5";

            public DriftScopeEntry(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Glob()
            {
                return _stereotype.GetProperty<string>("Glob");
            }

            public string Types()
            {
                return _stereotype.GetProperty<string>("Types");
            }

            public ScopeOptions Scope()
            {
                return new ScopeOptions(_stereotype.GetProperty<string>("Scope"));
            }

            public string Reason()
            {
                return _stereotype.GetProperty<string>("Reason");
            }

            public class ScopeOptions
            {
                public readonly string Value;

                public ScopeOptions(string value)
                {
                    Value = value;
                }

                public ScopeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "In":
                            return ScopeOptionsEnum.In;
                        case "Out":
                            return ScopeOptionsEnum.Out;
                        case "Context":
                            return ScopeOptionsEnum.Context;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsIn()
                {
                    return Value == "In";
                }
                public bool IsOut()
                {
                    return Value == "Out";
                }
                public bool IsContext()
                {
                    return Value == "Context";
                }
            }

            public enum ScopeOptionsEnum
            {
                In,
                Out,
                Context
            }
        }

    }
}