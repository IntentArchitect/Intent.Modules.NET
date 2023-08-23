using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Dapr.AspNetCore.Bindings.Cron.Api
{
    public static class CommandModelStereotypeExtensions
    {
        public static DaprCronBinding GetDaprCronBinding(this CommandModel model)
        {
            var stereotype = model.GetStereotype("Dapr Cron Binding");
            return stereotype != null ? new DaprCronBinding(stereotype) : null;
        }


        public static bool HasDaprCronBinding(this CommandModel model)
        {
            return model.HasStereotype("Dapr Cron Binding");
        }

        public static bool TryGetDaprCronBinding(this CommandModel model, out DaprCronBinding stereotype)
        {
            if (!HasDaprCronBinding(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DaprCronBinding(model.GetStereotype("Dapr Cron Binding"));
            return true;
        }

        public class DaprCronBinding
        {
            private IStereotype _stereotype;

            public DaprCronBinding(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Schedule()
            {
                return _stereotype.GetProperty<string>("Schedule");
            }

        }

    }
}