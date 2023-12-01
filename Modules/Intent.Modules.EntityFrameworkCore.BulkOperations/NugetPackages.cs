using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.EntityFrameworkCore.BulkOperations
{
    internal class NugetPackages
    {
        public static NugetPackageInfo ZEntityFrameworkExtensionsEFCore(IOutputTarget outputTarget) => new NugetPackageInfo("Z.EntityFramework.Extensions.EFCore", GetVersion(outputTarget.GetProject()));

        private static string GetVersion(ICSharpProject project)
        {
            return project switch
            {
                _ when project.IsNetApp(5) => "5.101.0",
                _ when project.IsNetApp(6) => "6.101.0",
                _ when project.IsNetApp(7) => "7.101.0",
                _ when project.IsNetApp(8) => "8.101.0",
                _ => throw new Exception("Not supported version of .NET Core")
            };
        }

    }
}
