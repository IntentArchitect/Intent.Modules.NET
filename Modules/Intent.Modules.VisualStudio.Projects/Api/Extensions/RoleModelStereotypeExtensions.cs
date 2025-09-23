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
    public static class RoleModelStereotypeExtensions
    {
        public static RoleSettings GetRoleSettings(this RoleModel model)
        {
            var stereotype = model.GetStereotype(RoleSettings.DefinitionId);
            return stereotype != null ? new RoleSettings(stereotype) : null;
        }


        public static bool HasRoleSettings(this RoleModel model)
        {
            return model.HasStereotype(RoleSettings.DefinitionId);
        }

        public static bool TryGetRoleSettings(this RoleModel model, out RoleSettings stereotype)
        {
            if (!HasRoleSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RoleSettings(model.GetStereotype(RoleSettings.DefinitionId));
            return true;
        }

        public class RoleSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "a23c7901-31a5-4cbf-b8bf-1be128977e6d";

            public RoleSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool CreateSubFolders()
            {
                return _stereotype.GetProperty<bool>("Create Sub-Folders");
            }

        }

    }
}