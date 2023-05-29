using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.HotChocolate.GraphQL.AzureFunctions
{
    public class NuGetPackages
    {
        public static INugetPackageInfo HotChocolate => new NugetPackageInfo("HotChocolate", "");
        public static INugetPackageInfo HotChocolateAzureFunctions => new NugetPackageInfo("HotChocolate.AzureFunctions", "13.1.0");
    }
}
