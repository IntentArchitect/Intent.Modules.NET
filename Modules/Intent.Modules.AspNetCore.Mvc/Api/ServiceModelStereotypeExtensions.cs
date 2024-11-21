using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.Mvc.Api
{
    public static class ServiceModelStereotypeExtensions
    {
        public static MVCSettings GetMVCSettings(this ServiceModel model)
        {
            var stereotype = model.GetStereotype(MVCSettings.DefinitionId);
            return stereotype != null ? new MVCSettings(stereotype) : null;
        }


        public static bool HasMVCSettings(this ServiceModel model)
        {
            return model.HasStereotype(MVCSettings.DefinitionId);
        }

        public static bool TryGetMVCSettings(this ServiceModel model, out MVCSettings stereotype)
        {
            if (!HasMVCSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MVCSettings(model.GetStereotype(MVCSettings.DefinitionId));
            return true;
        }

        public class MVCSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "7bf5b988-bcc6-4e1f-8384-093fa04cbba5";

            public MVCSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Route()
            {
                return _stereotype.GetProperty<string>("Route");
            }

        }

    }
}