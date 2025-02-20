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
            var stereotype = model.GetStereotype(ContractOnly.DefinitionId);
            return stereotype != null ? new ContractOnly(stereotype) : null;
        }


        public static bool HasContractOnly(this DomainServiceModel model)
        {
            return model.HasStereotype(ContractOnly.DefinitionId);
        }

        public static bool TryGetContractOnly(this DomainServiceModel model, out ContractOnly stereotype)
        {
            if (!HasContractOnly(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ContractOnly(model.GetStereotype(ContractOnly.DefinitionId));
            return true;
        }

        public static ServiceRegistrationSettings GetServiceRegistrationSettings(this DomainServiceModel model)
        {
            var stereotype = model.GetStereotype(ServiceRegistrationSettings.DefinitionId);
            return stereotype != null ? new ServiceRegistrationSettings(stereotype) : null;
        }


        public static bool HasServiceRegistrationSettings(this DomainServiceModel model)
        {
            return model.HasStereotype(ServiceRegistrationSettings.DefinitionId);
        }

        public static bool TryGetServiceRegistrationSettings(this DomainServiceModel model, out ServiceRegistrationSettings stereotype)
        {
            if (!HasServiceRegistrationSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ServiceRegistrationSettings(model.GetStereotype(ServiceRegistrationSettings.DefinitionId));
            return true;
        }

        public class ContractOnly
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "12fc5683-f818-43ea-a3c7-c843d4a40352";

            public ContractOnly(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class ServiceRegistrationSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "6ad19817-2014-464b-a525-409b3a5230d1";

            public ServiceRegistrationSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public ServiceRegistrationScopeOptions ServiceRegistrationScope()
            {
                return new ServiceRegistrationScopeOptions(_stereotype.GetProperty<string>("Service Registration Scope"));
            }

            public class ServiceRegistrationScopeOptions
            {
                public readonly string Value;

                public ServiceRegistrationScopeOptions(string value)
                {
                    Value = value;
                }

                public ServiceRegistrationScopeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Transient":
                            return ServiceRegistrationScopeOptionsEnum.Transient;
                        case "Scoped":
                            return ServiceRegistrationScopeOptionsEnum.Scoped;
                        case "Singleton":
                            return ServiceRegistrationScopeOptionsEnum.Singleton;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsTransient()
                {
                    return Value == "Transient";
                }
                public bool IsScoped()
                {
                    return Value == "Scoped";
                }
                public bool IsSingleton()
                {
                    return Value == "Singleton";
                }
            }

            public enum ServiceRegistrationScopeOptionsEnum
            {
                Transient,
                Scoped,
                Singleton
            }
        }

    }
}