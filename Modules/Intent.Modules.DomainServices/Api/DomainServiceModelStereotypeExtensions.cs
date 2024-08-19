using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.DomainServices.Api
{
    public static class DomainServiceModelStereotypeExtensions
    {
        public static ContractOnly GetContractOnly(this DomainServiceModel model)
        {
            var stereotype = model.GetStereotype("12fc5683-f818-43ea-a3c7-c843d4a40352");
            return stereotype != null ? new ContractOnly(stereotype) : null;
        }


        public static bool HasContractOnly(this DomainServiceModel model)
        {
            return model.HasStereotype("12fc5683-f818-43ea-a3c7-c843d4a40352");
        }

        public static bool TryGetContractOnly(this DomainServiceModel model, out ContractOnly stereotype)
        {
            if (!HasContractOnly(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ContractOnly(model.GetStereotype("12fc5683-f818-43ea-a3c7-c843d4a40352"));
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