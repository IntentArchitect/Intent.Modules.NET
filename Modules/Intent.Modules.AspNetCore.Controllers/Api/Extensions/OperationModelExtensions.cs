using System;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

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

            public VerbOptions Verb()
            {
                return new VerbOptions(_stereotype.GetProperty<string>("Verb"));
            }

            public string Route()
            {
                return _stereotype.GetProperty<string>("Route");
            }

            public class VerbOptions
            {
                public readonly string Value;

                public VerbOptions(string value)
                {
                    Value = value;
                }

                public bool IsDefault()
                {
                    return Value == "Default";
                }
                public bool IsHttpGet()
                {
                    return Value == "HttpGet";
                }
                public bool IsHttpPost()
                {
                    return Value == "HttpPost";
                }
                public bool IsHttpPut()
                {
                    return Value == "HttpPut";
                }
                public bool IsHttpDelete()
                {
                    return Value == "HttpDelete";
                }
            }

        }

    }
}