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
    public static class ApiGatewayRouteModelStereotypeExtensions
    {
        public static ApiGatewayRouteSettings GetApiGatewayRouteSettings(this ApiGatewayRouteModel model)
        {
            var stereotype = model.GetStereotype(ApiGatewayRouteSettings.DefinitionId);
            return stereotype != null ? new ApiGatewayRouteSettings(stereotype) : null;
        }


        public static bool HasApiGatewayRouteSettings(this ApiGatewayRouteModel model)
        {
            return model.HasStereotype(ApiGatewayRouteSettings.DefinitionId);
        }

        public static bool TryGetApiGatewayRouteSettings(this ApiGatewayRouteModel model, out ApiGatewayRouteSettings stereotype)
        {
            if (!HasApiGatewayRouteSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ApiGatewayRouteSettings(model.GetStereotype(ApiGatewayRouteSettings.DefinitionId));
            return true;
        }

        public class ApiGatewayRouteSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1ad354a7-9ef2-440f-acb2-d9cb11be3352";

            public ApiGatewayRouteSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public HttpMethodOptions HttpMethod()
            {
                return new HttpMethodOptions(_stereotype.GetProperty<string>("Http Method"));
            }

            public string UpstreamPath()
            {
                return _stereotype.GetProperty<string>("Upstream Path");
            }

            public class HttpMethodOptions
            {
                public readonly string Value;

                public HttpMethodOptions(string value)
                {
                    Value = value;
                }

                public HttpMethodOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "GET":
                            return HttpMethodOptionsEnum.GET;
                        case "POST":
                            return HttpMethodOptionsEnum.POST;
                        case "PUT":
                            return HttpMethodOptionsEnum.PUT;
                        case "PATCH":
                            return HttpMethodOptionsEnum.PATCH;
                        case "DELETE":
                            return HttpMethodOptionsEnum.DELETE;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsGET()
                {
                    return Value == "GET";
                }
                public bool IsPOST()
                {
                    return Value == "POST";
                }
                public bool IsPUT()
                {
                    return Value == "PUT";
                }
                public bool IsPATCH()
                {
                    return Value == "PATCH";
                }
                public bool IsDELETE()
                {
                    return Value == "DELETE";
                }
            }

            public enum HttpMethodOptionsEnum
            {
                GET,
                POST,
                PUT,
                PATCH,
                DELETE
            }
        }

    }
}