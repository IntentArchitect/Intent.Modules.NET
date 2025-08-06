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
            //This is base component library, if you have this installed you have a component library like MudBlazor installed
            return application.InstalledModules.Any(x => x.ModuleId == "Intent.Modelers.UI.Core");
        }

    }
}
