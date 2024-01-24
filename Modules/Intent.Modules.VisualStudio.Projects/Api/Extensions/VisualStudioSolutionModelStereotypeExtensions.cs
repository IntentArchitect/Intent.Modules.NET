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
    public static class VisualStudioSolutionModelStereotypeExtensions
    {
        public static VisualStudioSolutionOptions GetVisualStudioSolutionOptions(this VisualStudioSolutionModel model)
        {
            var stereotype = model.GetStereotype("90085088-b29b-4386-b87e-f7f62c76c5de");
            return stereotype != null ? new VisualStudioSolutionOptions(stereotype) : null;
        }


        public static bool HasVisualStudioSolutionOptions(this VisualStudioSolutionModel model)
        {
            return model.HasStereotype("90085088-b29b-4386-b87e-f7f62c76c5de");
        }

        public static bool TryGetVisualStudioSolutionOptions(this VisualStudioSolutionModel model, out VisualStudioSolutionOptions stereotype)
        {
            if (!HasVisualStudioSolutionOptions(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new VisualStudioSolutionOptions(model.GetStereotype("90085088-b29b-4386-b87e-f7f62c76c5de"));
            return true;
        }

        public class VisualStudioSolutionOptions
        {
            private IStereotype _stereotype;

            public VisualStudioSolutionOptions(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public bool ManagePackageVersionsCentrally()
            {
                return _stereotype.GetProperty<bool>("Manage Package Versions Centrally");
            }

        }

    }
}