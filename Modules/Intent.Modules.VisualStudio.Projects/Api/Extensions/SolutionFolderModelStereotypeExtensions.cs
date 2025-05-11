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
    public static class SolutionFolderModelStereotypeExtensions
    {
        public static FolderOptions GetFolderOptions(this SolutionFolderModel model)
        {
            var stereotype = model.GetStereotype(FolderOptions.DefinitionId);
            return stereotype != null ? new FolderOptions(stereotype) : null;
        }


        public static bool HasFolderOptions(this SolutionFolderModel model)
        {
            return model.HasStereotype(FolderOptions.DefinitionId);
        }

        public static bool TryGetFolderOptions(this SolutionFolderModel model, out FolderOptions stereotype)
        {
            if (!HasFolderOptions(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FolderOptions(model.GetStereotype(FolderOptions.DefinitionId));
            return true;
        }

        public class FolderOptions
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "bb59d570-80b6-4564-9016-f809d6eacbc5";

            public FolderOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool MaterializeFolder()
            {
                return _stereotype.GetProperty<bool>("Materialize Folder");
            }

        }

    }
}