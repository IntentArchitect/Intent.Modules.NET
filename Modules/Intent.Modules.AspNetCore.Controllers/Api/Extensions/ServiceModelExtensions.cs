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
        public static ControllerSettings GetControllerSettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("Controller Settings");
            return stereotype != null ? new ControllerSettings(stereotype) : null;
        }

        public static bool HasControllerSettings(this ServiceModel model)
        {
            return model.HasStereotype("Controller Settings");
        }


        public class ControllerSettings
        {
            private IStereotype _stereotype;

            public ControllerSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Route()
            {
                return _stereotype.GetProperty<string>("Route");
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