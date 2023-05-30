using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Dapr.AspNetCore.StateManagement.Api
{
    public static class StateStoreModelStereotypeExtensions
    {
        public static Settings GetSettings(this StateStoreModel model)
        {
            var stereotype = model.GetStereotype("Settings");
            return stereotype != null ? new Settings(stereotype) : null;
        }


        public static bool HasSettings(this StateStoreModel model)
        {
            return model.HasStereotype("Settings");
        }

        public static bool TryGetSettings(this StateStoreModel model, out Settings stereotype)
        {
            if (!HasSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Settings(model.GetStereotype("Settings"));
            return true;
        }

        public class Settings
        {
            private IStereotype _stereotype;

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public bool EnableTransactions()
            {
                return _stereotype.GetProperty<bool>("Enable Transactions");
            }

        }

    }
}