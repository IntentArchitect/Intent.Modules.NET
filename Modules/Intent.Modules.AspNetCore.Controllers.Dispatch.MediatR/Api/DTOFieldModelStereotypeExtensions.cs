using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.Controllers.Dispatch.MediatR.Api
{
    public static class DTOFieldModelStereotypeExtensions
    {
        public static ParameterSettings GetParameterSettings(this DTOFieldModel model)
        {
            var stereotype = model.GetStereotype("Parameter Settings");
            return stereotype != null ? new ParameterSettings(stereotype) : null;
        }


        public static bool HasParameterSettings(this DTOFieldModel model)
        {
            return model.HasStereotype("Parameter Settings");
        }

        public static bool TryGetParameterSettings(this DTOFieldModel model, out ParameterSettings stereotype)
        {
            if (!HasParameterSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ParameterSettings(model.GetStereotype("Parameter Settings"));
            return true;
        }

        public class ParameterSettings
        {
            private IStereotype _stereotype;

            public ParameterSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public SourceOptions Source()
            {
                return new SourceOptions(_stereotype.GetProperty<string>("Source"));
            }

            public string HeaderName()
            {
                return _stereotype.GetProperty<string>("Header Name");
            }

            public class SourceOptions
            {
                public readonly string Value;

                public SourceOptions(string value)
                {
                    Value = value;
                }

                public SourceOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Default":
                            return SourceOptionsEnum.Default;
                        case "From Body":
                            return SourceOptionsEnum.FromBody;
                        case "From Form":
                            return SourceOptionsEnum.FromForm;
                        case "From Header":
                            return SourceOptionsEnum.FromHeader;
                        case "From Query":
                            return SourceOptionsEnum.FromQuery;
                        case "From Route":
                            return SourceOptionsEnum.FromRoute;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDefault()
                {
                    return Value == "Default";
                }
                public bool IsFromBody()
                {
                    return Value == "From Body";
                }
                public bool IsFromForm()
                {
                    return Value == "From Form";
                }
                public bool IsFromHeader()
                {
                    return Value == "From Header";
                }
                public bool IsFromQuery()
                {
                    return Value == "From Query";
                }
                public bool IsFromRoute()
                {
                    return Value == "From Route";
                }
            }

            public enum SourceOptionsEnum
            {
                Default,
                FromBody,
                FromForm,
                FromHeader,
                FromQuery,
                FromRoute
            }
        }

    }
}