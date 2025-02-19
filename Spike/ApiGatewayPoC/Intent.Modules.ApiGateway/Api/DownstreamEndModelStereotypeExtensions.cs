using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.ApiGateway.Api
{
    public static class DownstreamEndModelStereotypeExtensions
    {
        public static DownstreamRouteSettings GetDownstreamRouteSettings(this DownstreamEndModel model)
        {
            var stereotype = model.GetStereotype(DownstreamRouteSettings.DefinitionId);
            return stereotype != null ? new DownstreamRouteSettings(stereotype) : null;
        }


        public static bool HasDownstreamRouteSettings(this DownstreamEndModel model)
        {
            return model.HasStereotype(DownstreamRouteSettings.DefinitionId);
        }

        public static bool TryGetDownstreamRouteSettings(this DownstreamEndModel model, out DownstreamRouteSettings stereotype)
        {
            if (!HasDownstreamRouteSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DownstreamRouteSettings(model.GetStereotype(DownstreamRouteSettings.DefinitionId));
            return true;
        }

        public class DownstreamRouteSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5b275faf-da10-4ba2-b3f4-bd2c3c86d1e4";

            public DownstreamRouteSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public SchemeOptions Scheme()
            {
                return new SchemeOptions(_stereotype.GetProperty<string>("Scheme"));
            }

            public class SchemeOptions
            {
                public readonly string Value;

                public SchemeOptions(string value)
                {
                    Value = value;
                }

                public SchemeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "HTTP":
                            return SchemeOptionsEnum.HTTP;
                        case "HTTPS":
                            return SchemeOptionsEnum.HTTPS;
                        case "WS":
                            return SchemeOptionsEnum.WS;
                        case "WSS":
                            return SchemeOptionsEnum.WSS;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsHTTP()
                {
                    return Value == "HTTP";
                }
                public bool IsHTTPS()
                {
                    return Value == "HTTPS";
                }
                public bool IsWS()
                {
                    return Value == "WS";
                }
                public bool IsWSS()
                {
                    return Value == "WSS";
                }
            }

            public enum SchemeOptionsEnum
            {
                HTTP,
                HTTPS,
                WS,
                WSS
            }
        }

    }
}