using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.Controllers.Api
{
    public static class OperationModelExtensions
    {
        public static ApiSettings GetApiSettings(this OperationModel model)
        {
            var stereotype = model.GetStereotype("Api Settings");
            return stereotype != null ? new ApiSettings(stereotype) : null;
        }

        public static bool HasApiSettings(this OperationModel model)
        {
            return model.HasStereotype("Api Settings");
        }


        public class ApiSettings
        {
            private IStereotype _stereotype;

            public ApiSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public HttpVerbOptions HttpVerb()
            {
                return new HttpVerbOptions(_stereotype.GetProperty<string>("Http Verb"));
            }

            public string HttpRoute()
            {
                return _stereotype.GetProperty<string>("Http Route");
            }

            public SecurityOptions Security()
            {
                return new SecurityOptions(_stereotype.GetProperty<string>("Security"));
            }

            public class HttpVerbOptions
            {
                public readonly string Value;

                public HttpVerbOptions(string value)
                {
                    Value = value;
                }

                public bool IsAUTO()
                {
                    return Value == "AUTO";
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
                public bool IsDELETE()
                {
                    return Value == "DELETE";
                }
            }

            public class SecurityOptions
            {
                public readonly string Value;

                public SecurityOptions(string value)
                {
                    Value = value;
                }

                public bool IsInherit()
                {
                    return Value == "Inherit";
                }
                public bool IsAuthorize()
                {
                    return Value == "Authorize";
                }
                public bool IsAllowAnonymous()
                {
                    return Value == "Allow Anonymous";
                }
            }

        }

    }
}