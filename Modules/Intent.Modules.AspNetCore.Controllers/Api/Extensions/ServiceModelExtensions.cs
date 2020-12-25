using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.Controllers.Api
{
    public static class ServiceModelExtensions
    {
        public static ApiSettings GetApiSettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("Api Settings");
            return stereotype != null ? new ApiSettings(stereotype) : null;
        }

        public static bool HasApiSettings(this ServiceModel model)
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

            public string HttpRoute()
            {
                return _stereotype.GetProperty<string>("Http Route");
            }

            public SecurityOptions Security()
            {
                return new SecurityOptions(_stereotype.GetProperty<string>("Security"));
            }

            public class SecurityOptions
            {
                public readonly string Value;

                public SecurityOptions(string value)
                {
                    Value = value;
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