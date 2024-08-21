using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.AspNetCore.SignalR.Api
{
    public static class SignalRHubModelStereotypeExtensions
    {
        public static HubSettings GetHubSettings(this SignalRHubModel model)
        {
            var stereotype = model.GetStereotype("ba101028-d276-496d-bab2-5e7b707449a4");
            return stereotype != null ? new HubSettings(stereotype) : null;
        }


        public static bool HasHubSettings(this SignalRHubModel model)
        {
            return model.HasStereotype("ba101028-d276-496d-bab2-5e7b707449a4");
        }

        public static bool TryGetHubSettings(this SignalRHubModel model, out HubSettings stereotype)
        {
            if (!HasHubSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HubSettings(model.GetStereotype("ba101028-d276-496d-bab2-5e7b707449a4"));
            return true;
        }

        public class HubSettings
        {
            private IStereotype _stereotype;

            public HubSettings(IStereotype stereotype)
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