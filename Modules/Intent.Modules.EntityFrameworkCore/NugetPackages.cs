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
        public static NugetPackageInfo NpgsqlEntityFrameworkCorePostgreSQL(IOutputTarget outputTarget) => new NugetPackageInfo("Npgsql.EntityFrameworkCore.PostgreSQL", GetPostgreSqlVersion(outputTarget.GetProject()));
        public static NugetPackageInfo EntityFrameworkCoreCosmos(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore.Cosmos", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo EntityFrameworkCoreInMemory(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore.InMemory", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo EntityFrameworkCoreProxies(IOutputTarget outputTarget) => new NugetPackageInfo("Microsoft.EntityFrameworkCore.Proxies", GetVersion(outputTarget.GetProject()));
        public static NugetPackageInfo MySqlEntityFrameworkCore(IOutputTarget outputTarget) => new NugetPackageInfo("Pomelo.EntityFrameworkCore.MySql", GetMySqlVersion(outputTarget.GetProject()));

        private static string GetPostgreSqlVersion(ICSharpProject project)
        {
            return project switch
            {
                _ when project.IsNetApp(5) => "5.0.10",
                _ when project.IsNetApp(6) => "6.0.8",
                _ when project.IsNetApp(7) => "7.0.4",
                _ when project.IsNetApp(8) => "8.0.0",
                _ => throw new Exception("Not supported version of .NET Core") 
            };
        }

        private static string GetMySqlVersion(ICSharpProject project)
        {
            return project switch
            {
                _ when project.IsNetApp(5) => "5.0.4",
                _ when project.IsNetApp(6) => "6.0.2",
                _ when project.IsNetApp(7) => "7.0.0",
                _ when project.IsNetApp(8) => "7.0.0",
                _ => throw new Exception("Not supported version of .NET Core") 
            };
        }

        private static string GetVersion(ICSharpProject project)
        {
            return project switch
            {
                _ when project.IsNetCore2App() => throw new Exception(".NET Core 2.x is no longer supported."),
                _ when project.IsNetCore3App() => "3.1.24",
                _ when project.IsNetApp(5) => "5.0.17",
                _ when project.IsNetApp(6) => "6.0.20",
                _ when project.IsNetApp(7) => "7.0.9",
                _ when project.IsNetApp(8) => "8.0.0",
                _ => "6.0.20"
            };
        }
    }
}
