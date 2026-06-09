using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Blazor.Templates
{
    internal class TemplateHelper
    {
        internal static bool ComponentLibraryInstalled(IApplication application)
        {
            // This is the base component library marker; component-library modules install it.
            return application.InstalledModules.Any(x => x.ModuleId == "Intent.Modelers.UI.Core");
        }

    }
}
