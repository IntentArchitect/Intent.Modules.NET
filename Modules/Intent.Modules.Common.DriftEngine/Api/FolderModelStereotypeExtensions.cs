using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Common.DriftEngine.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static DriftScope GetDriftScope(this FolderModel model)
        {
            var stereotype = model.GetStereotype(DriftScope.DefinitionId);
            return stereotype != null ? new DriftScope(stereotype) : null;
        }


        public static bool HasDriftScope(this FolderModel model)
        {
            return model.HasStereotype(DriftScope.DefinitionId);
        }

        public static bool TryGetDriftScope(this FolderModel model, out DriftScope stereotype)
        {
            if (!HasDriftScope(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DriftScope(model.GetStereotype(DriftScope.DefinitionId));
            return true;
        }

        public static DriftScopeEntry GetDriftScopeEntry(this FolderModel model)
        {
            var stereotype = model.GetStereotype(DriftScopeEntry.DefinitionId);
            return stereotype != null ? new DriftScopeEntry(stereotype) : null;
        }


        public static bool HasDriftScopeEntry(this FolderModel model)
        {
            return model.HasStereotype(DriftScopeEntry.DefinitionId);
        }

        public static bool TryGetDriftScopeEntry(this FolderModel model, out DriftScopeEntry stereotype)
        {
            if (!HasDriftScopeEntry(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DriftScopeEntry(model.GetStereotype(DriftScopeEntry.DefinitionId));
            return true;
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
            }

            public enum ScopeOptionsEnum
            {
                In,
                Out
            }
        }

    }
}