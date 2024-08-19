using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Dapr.AspNetCore.StateManagement.Api
{
    public static class DomainPackageModelStereotypeExtensions
    {
        public static DaprStateStoreSettings GetDaprStateStoreSettings(this DomainPackageModel model)
        {
            var stereotype = model.GetStereotype("e707106e-0496-4717-8d50-402e84aa3d39");
            return stereotype != null ? new DaprStateStoreSettings(stereotype) : null;
        }


        public static bool HasDaprStateStoreSettings(this DomainPackageModel model)
        {
            return model.HasStereotype("e707106e-0496-4717-8d50-402e84aa3d39");
        }

        public static bool TryGetDaprStateStoreSettings(this DomainPackageModel model, out DaprStateStoreSettings stereotype)
        {
            if (!HasDaprStateStoreSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DaprStateStoreSettings(model.GetStereotype("e707106e-0496-4717-8d50-402e84aa3d39"));
            return true;
        }

        public class DaprStateStoreSettings
        {
            private IStereotype _stereotype;

            public DaprStateStoreSettings(IStereotype stereotype)
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