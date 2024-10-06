using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public static class NETFrameworkVersionModelStereotypeExtensions
    {
        public static NETFrameworkVersionSettings GetNETFrameworkVersionSettings(this NETFrameworkVersionModel model)
        {
            var stereotype = model.GetStereotype(NETFrameworkVersionSettings.DefinitionId);
            return stereotype != null ? new NETFrameworkVersionSettings(stereotype) : null;
        }

        public static bool HasNETFrameworkVersionSettings(this NETFrameworkVersionModel model)
        {
            return model.HasStereotype(NETFrameworkVersionSettings.DefinitionId);
        }

        public static bool TryGetNETFrameworkVersionSettings(this NETFrameworkVersionModel model, out NETFrameworkVersionSettings stereotype)
        {
            if (!HasNETFrameworkVersionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new NETFrameworkVersionSettings(model.GetStereotype(NETFrameworkVersionSettings.DefinitionId));
            return true;
        }


        public class NETFrameworkVersionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "21cd7c00-cf82-4526-8539-8685dfca3791";

            public NETFrameworkVersionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string LegacyVersionIdentifier()
            {
                return _stereotype.GetProperty<string>("Legacy Version Identifier");
            }

        }

    }
}