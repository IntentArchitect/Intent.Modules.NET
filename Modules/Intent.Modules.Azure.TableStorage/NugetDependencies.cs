using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Azure.TableStorage
{
    internal class NugetDependencies
    {
        public static readonly INugetPackageInfo AzureDataTables = new NugetPackageInfo("Azure.Data.Tables", "12.8.1");
    }
}
