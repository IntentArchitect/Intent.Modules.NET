using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Application.ServiceImplementations.Api
{
    public static class ServiceModelStereotypeExtensions
    {
        public static ContractOnly GetContractOnly(this ServiceModel model)
        {
            var stereotype = model.GetStereotype("f7a2e653-d654-48d6-9a44-18cf49c9233e");
            return stereotype != null ? new ContractOnly(stereotype) : null;
        }


        public static bool HasContractOnly(this ServiceModel model)
        {
            return model.HasStereotype("f7a2e653-d654-48d6-9a44-18cf49c9233e");
        }

        public static bool TryGetContractOnly(this ServiceModel model, out ContractOnly stereotype)
        {
            if (!HasContractOnly(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ContractOnly(model.GetStereotype("f7a2e653-d654-48d6-9a44-18cf49c9233e"));
            return true;
        }

        public class ContractOnly
        {
            private IStereotype _stereotype;

            public ContractOnly(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}