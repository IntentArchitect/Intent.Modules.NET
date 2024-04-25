using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.OutputCaching.Redis.Api
{
    public static class FolderModelStereotypeExtensions
    {
        public static Caching GetCaching(this FolderModel model)
        {
            var stereotype = model.GetStereotype("c090804e-8e1c-4121-8209-488982ce6a22");
            return stereotype != null ? new Caching(stereotype) : null;
        }


        public static bool HasCaching(this FolderModel model)
        {
            return model.HasStereotype("c090804e-8e1c-4121-8209-488982ce6a22");
        }

        public static bool TryGetCaching(this FolderModel model, out Caching stereotype)
        {
            if (!HasCaching(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Caching(model.GetStereotype("c090804e-8e1c-4121-8209-488982ce6a22"));
            return true;
        }

        public class Caching
        {
            private IStereotype _stereotype;

            public Caching(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement Policy()
            {
                return _stereotype.GetProperty<IElement>("Policy");
            }

            public bool Override()
            {
                return _stereotype.GetProperty<bool>("Override");
            }

            public int? Duration()
            {
                return _stereotype.GetProperty<int?>("Duration");
            }

            public string Tags()
            {
                return _stereotype.GetProperty<string>("Tags");
            }

            public string VaryByQueryKeys()
            {
                return _stereotype.GetProperty<string>("Vary By Query Keys");
            }

            public string VaryByHeaderNames()
            {
                return _stereotype.GetProperty<string>("Vary By Header Names");
            }

            public string VaryByRouteValueNames()
            {
                return _stereotype.GetProperty<string>("Vary By Route Value Names");
            }

            public bool NoCaching()
            {
                return _stereotype.GetProperty<bool>("No Caching");
            }

            public class OverrideOptions
            {
                public readonly string Value;

                public OverrideOptions(string value)
                {
                    Value = value;
                }

                public OverrideOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Policy":
                            return OverrideOptionsEnum.Policy;
                        case "Custom":
                            return OverrideOptionsEnum.Custom;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsPolicy()
                {
                    return Value == "Policy";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

            public enum OverrideOptionsEnum
            {
                Policy,
                Custom
            }
        }

    }
}