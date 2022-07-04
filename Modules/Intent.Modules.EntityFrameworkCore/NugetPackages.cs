using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore
{
    public static class NugetPackages
    {
        public static NugetPackageInfo EntityFrameworkCore(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo EntityFrameworkCoreDesign(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore.Design", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo EntityFrameworkCoreTools(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore.Tools", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo EntityFrameworkCoreSqlServer(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore.SqlServer", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo NpgsqlEntityFrameworkCorePostgreSQL(IOutputTarget outputTarget) => new NugetPackageInfo("Npgsql.EntityFrameworkCore.PostgreSQL", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo EntityFrameworkCoreCosmos(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore.Cosmos", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo EntityFrameworkCoreInMemory(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore.InMemory", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo EntityFrameworkCoreProxies(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore.Proxies", GetVersion(outputTarget.GetProject()));

        private static string GetVersion(ICSharpProject project)
        {
            if (project.IsNetCore2App())
            {
                throw new Exception(".NET Core 2.x is no longer supported.");
            }
            if (project.IsNetCore3App())
            {
                return "3.1.24";
            }
            if (project.IsNet5App())
            {
                return "5.0.16";
            }
            if (project.IsNet6App())
            {
                return "6.0.4";
            }
            return "6.0.4";
        }
    }
}
