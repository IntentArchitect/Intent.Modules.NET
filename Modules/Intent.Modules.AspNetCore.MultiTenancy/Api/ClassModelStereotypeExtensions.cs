using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Api
{
    public static class ClassModelStereotypeExtensions
    {
        public static MultiTenant GetMultiTenant(this ClassModel model)
        {
            var stereotype = model.GetStereotype(MultiTenant.DefinitionId);
            return stereotype != null ? new MultiTenant(stereotype) : null;
        }


        public static bool HasMultiTenant(this ClassModel model)
        {
            return model.HasStereotype(MultiTenant.DefinitionId);
        }

        public static bool TryGetMultiTenant(this ClassModel model, out MultiTenant stereotype)
        {
            if (!HasMultiTenant(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MultiTenant(model.GetStereotype(MultiTenant.DefinitionId));
            return true;
        }

        public class MultiTenant
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "586eb05b-d647-4430-ac05-8d096fe3f79e";

            public MultiTenant(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}